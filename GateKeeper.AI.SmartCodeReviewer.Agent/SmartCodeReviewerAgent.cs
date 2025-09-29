#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Azure.AI.Agents.Persistent;
using Azure.Identity;
using GateKeeper.AI.Shared;
using GateKeeper.AI.Shared.MCP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using System.Threading.RateLimiting;
using static GateKeeper.AI.Shared.Settings;

namespace GateKeeper.AI.SmartCodeReviewer.Agent;

public interface ISmartCodeReviewerAgentDefinition
{
    Task<(Kernel, AzureAIAgent, AzureAIAgentThread)> CreateAgent();
}

public class SmartCodeReviewerAgentDefinition : ISmartCodeReviewerAgentDefinition
{
    private readonly Settings _settings;
    private readonly IMcpClientHost _mcpClient;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly FixedWindowRateLimiter _rateLimiter;
    public SmartCodeReviewerAgentDefinition(Settings settings,IMcpClientHost mcpClient)
    {
        _settings = settings;
        _mcpClient = mcpClient;
        var retryDelays = Backoff.ExponentialBackoff(
        TimeSpan.FromSeconds(1), retryCount: 5);
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<Azure.RequestFailedException>(ex => ex.Status == 429) // Azure throttling
            .WaitAndRetryAsync(retryDelays,
                (exception, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"⚠️ Rate limited. Retrying in {timespan.TotalSeconds} sec (Attempt {retryCount})");
                });
         _rateLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromSeconds(5),
            QueueLimit = 20,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    }

    public async Task<(Kernel,AzureAIAgent,AzureAIAgentThread)> CreateAgent()
    {
        var builder = Kernel.CreateBuilder();

        AzureOpenAIPromptExecutionSettings executionSettings = new()
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }, autoInvoke: true),
        };
        var credentials = new DefaultAzureCredential();
        PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient(_settings.Foundry.Endpoint, credentials);
        PersistentAgent definition = await client.Administration.CreateAgentAsync(
            _settings.AzureOpenAI.ChatModelDeployment,
            name: "Smart Code Reviewer Agent",
            description: "Smart Code Reviewer Agent ",
            instructions: """
            
            You are a helpful github agent. 
            Runs post-merge checks for code quality issues.Detects dead code, long functions, poor naming, and anti-patterns.
            Base URL GitHub owner: https://github.com/AIS-IDC-Hackathon-2025/
            Organization: {githubSettings.Owner}
            Repository: {githubSettings.Repo}
                        
            """.Replace("{githubSettings.Owner}", _settings.GitSettings.Owner).Replace("{githubSettings.Repo}", _settings.GitSettings.Repo));
            
        await _mcpClient.Create();
        var tools = await _mcpClient.GetTools();
        var kernel = builder.Build();
        kernel.Plugins.AddFromFunctions("GitHubCopilot", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

        AzureAIAgent agent = new(definition, client, plugins: kernel.Plugins);
        AzureAIAgentThread agentThread = new(agent.Client);

        return (kernel, agent, agentThread);
        #pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    }
}
#pragma warning restore SKEXP0001
