using GateKeeper.AI.Shared;
using GateKeeper.AI.Shared.Hub;
using GateKeeper.AI.Shared.Plugin;
using GateKeeper.AI.SmartCodeReviewer.Agent;
using GateKeeper.AI.TagAndChangeLog.Agent;
using GateKeeper.AI.Trust.Agent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace GateKeeper.AI.Orchestrator;

public interface IOrchestratorService
{
    void InitializeKernels();

    Task RunTaggingAndChangeLogAsync(string message);

    Task RunTrustAgentAsync(string message);

    Task RunSmartCodeReviewAsync(string message);

    Task Create(string input, CancellationToken cancellationToken = default);
}
public class OrchestratorService(
    IHubContext<AgentsHub> hubContext,
    ITagAndChangeLogAgentDefinition tagAgent,
    ITrustAgent trustAgent,
    ISmartCodeReviewerAgentDefinition smartCRAgent,
    ILoggerFactory loggerFactory,
    Settings setting) : IOrchestratorService
{
    private OrchestrationMonitor monitor = null!;
    private Kernel? _managerKernel;
    private Kernel? _tagKernel;
    private Kernel? _trustKernel;
    private Kernel? _crKernel;
    private StandardMagenticManager? _manager;
    private MagenticOrchestration? _orchestration;


    public void InitializeKernels()
    {
        var builder = Kernel.CreateBuilder();

        builder.Services.AddSingleton(loggerFactory);
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: setting.AzureOpenAI.ChatModelDeployment,
            endpoint: setting.AzureOpenAI.Endpoint,
            apiKey: setting.AzureOpenAI.ApiKey);

        GitHubPlugin githubPlugin = new(setting);

        builder.Plugins.AddFromObject(githubPlugin);

        _managerKernel = builder.Build();
    }

    //public async Task InitializeOrchestration()
    //{
        
    //}

    public async Task RunTaggingAndChangeLogAsync(string message)
    {
        var (kernel, agent) = await tagAgent.CreateAgent();
        ChatHistoryAgentThread agentThread = new();
        await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread))
        {
            // Display response.
            Console.WriteLine($"{response.Content}");

            await hubContext.Clients.All.SendAsync("ReceiveMessage", response.Content);
        }
    }

    public async Task RunTrustAgentAsync(string message)
    {
        var chatCompletionAgent = trustAgent.CreateAgent(_managerKernel!);
        ChatHistoryAgentThread trustAgentThread = new();
        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(message, trustAgentThread))
        {
            // Display response.
            Console.WriteLine($"{response.Content}");

            await hubContext.Clients.All.SendAsync("ReceiveMessage", response.Content);
        }
    }

    public async Task RunSmartCodeReviewAsync(string message)
    {
        var (kernel, agent, thread) = await smartCRAgent.CreateAgent();
        await foreach (ChatMessageContent response in agent.InvokeAsync(message, thread))
        {
            // Display response.
            Console.WriteLine($"{response.Content}");

            await hubContext.Clients.All.SendAsync("ReceiveMessage", response.Content);
        }
    }

    public async Task Create(string input, CancellationToken cancellationToken = default)
    {
        var builder = Kernel.CreateBuilder();

        builder.Services.AddSingleton(loggerFactory);
        var tagAndChangeLogAgent = await tagAgent.CreateAgent();

        _tagKernel = tagAndChangeLogAgent.Item1;
        var crAgent = await smartCRAgent.CreateAgent();

        _crKernel = crAgent.Item1;

        monitor = new();

        builder.AddAzureOpenAIChatCompletion(
            deploymentName: setting.AzureOpenAI.ChatModelDeployment,
            endpoint: setting.AzureOpenAI.Endpoint,
            apiKey: setting.AzureOpenAI.ApiKey);

        _managerKernel = builder.Build();


        var executionSettings = new AzureOpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }, autoInvoke: true),
        };

        _manager = new(_managerKernel.GetRequiredService<IChatCompletionService>(), new AzureOpenAIPromptExecutionSettings())
        {
            MaximumInvocationCount = 5,
        };

        //
        //
        //, tagAndChangeLogAgent.Item2 
        _orchestration = new(_manager, crAgent.Item2)
        {
            LoggerFactory = loggerFactory,
            ResponseCallback = monitor.ResponseCallback,
           
        };
        // Start the runtime
        InProcessRuntime runtime = new();
        await runtime.StartAsync(cancellationToken);

        try
        {
            Console.WriteLine("Starting orchestration...");
            OrchestrationResult<string> result = await _orchestration.InvokeAsync(input, runtime);

            // Increase timeout to 5 minutes for complex orchestrations
            // Note: GetValueAsync only accepts TimeSpan, so we'll use the timeout parameter
            // The cancellation token is handled at the orchestration level
            cancellationToken.ThrowIfCancellationRequested();

            string text = await result.GetValueAsync(TimeSpan.FromMinutes(120));
            Console.WriteLine($"\n# RESULT: {text}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Orchestration timed out: {ex.Message}");
            Console.WriteLine("Consider breaking down the task into smaller parts or increasing timeout.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Orchestration failed: {ex.Message}");
            throw;
        }

        await runtime.RunUntilIdleAsync();

        Console.WriteLine("\n\nORCHESTRATION HISTORY");
        foreach (ChatMessageContent message in monitor.History)
        {
            //this.WriteAgentChatMessage(message);
            Console.WriteLine(message);
        }

    }

    public Task TestAgent(string message) => throw new NotImplementedException();
}
