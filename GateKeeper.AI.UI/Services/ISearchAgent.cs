using System.Threading.Tasks;
using Microsoft.AgentFramework.Core.Agents;
using Microsoft.AgentFramework.Core.Workflows;
using Microsoft.AgentFramework.Core.Steps;
using System.Collections.Generic;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Linq;

namespace GateKeeper.AI.UI.Services;

public interface ISearchAgent : IAgent
{
    Task<string> ExecuteSearch(string query);
}

// Agent for complex analysis
public interface IAnalyzerAgent : IAgent
{
    Task<string> AnalyzeData(string data);
}

// --- 2. Agent Implementations ---

// Concrete implementation of the Search Agent
public class SearchAgent : ISearchAgent
{
    public Task<string> ExecuteSearch(string query)
    {
        // Simulate an external API call or database query that takes time
        Task.Delay(3000).Wait(); // Simulating 3 seconds of work
        return Task.FromResult($"Search Result for '{query}': Found 15 relevant articles on concurrency and agents.");
    }
}

// Concrete implementation of the Analyzer Agent
public class AnalyzerAgent : IAnalyzerAgent
{
    public Task<string> AnalyzeData(string query)
    {
        // Simulate a heavy computational or model analysis task
        Task.Delay(5000).Wait(); // Simulating 5 seconds of work (Runs concurrently with SearchAgent)
        return Task.FromResult($"Analysis Report for '{query}': High impact, high risk, requires further review by a human.");
    }
}

// --- 3. Workflow Service and Orchestration ---

public class AgentWorkflowService
{
    private readonly ISearchAgent _searchAgent;
    private readonly IAnalyzerAgent _analyzerAgent;

    public AgentWorkflowService()
    {
        // Initialize Agents (in a real app, these would be injected or configured via a factory)
        _searchAgent = new SearchAgent();
        _analyzerAgent = new AnalyzerAgent();
    }

    /// <summary>
    /// Executes a concurrent workflow where two agents run in parallel.
    /// </summary>
    /// <param name="query">The user query to be passed to both parallel tasks.</param>
    /// <returns>A string representing the combined result of the concurrent steps.</returns>
    public async Task<string> RunConcurrentWorkflow(string query)
    {
        // 1. Define the Step that runs the search agent
        var searchStep = new AgentFunctionStep<ISearchAgent, string>(
            _searchAgent,
            nameof(_searchAgent.ExecuteSearch),
            new Dictionary<string, object> { { "query", query } }
        );

        // 2. Define the Step that runs the analyzer agent
        var analysisStep = new AgentFunctionStep<IAnalyzerAgent, string>(
            _analyzerAgent,
            nameof(_analyzerAgent.AnalyzeData),
            new Dictionary<string, object> { { "data", query } }
        );

        // 3. Define the ConcurrentStep to execute both steps in parallel
        var concurrentStep = new ConcurrentStep(new List<Step> { searchStep, analysisStep });

        // 4. Define a final step to combine the results (a simple LambdaStep)
        var combinationStep = new LambdaStep<string, string>(async (context, token) =>
        {
            // The results of the previous (concurrent) step are available in the context.
            // We need to retrieve the results from the Search and Analysis agents.

            var searchResult = context.GetStepOutput<string>(searchStep.Name);
            var analysisResult = context.GetStepOutput<string>(analysisStep.Name);

            // Combine the parallel results into a final output string
            return $"--- Final Combined Report ---\n\n" +
                   $"Search Output:\n{searchResult}\n\n" +
                   $"Analysis Output:\n{analysisResult}\n";

        }, "CombinationStep");

        // 5. Build the Workflow (ConcurrentStep runs first, followed by CombinationStep)
        var workflow = new Workflow("ConcurrentAnalysisWorkflow", new List<Step> { concurrentStep, combinationStep });

        // 6. Execute the Workflow
        var result = await workflow.ExecuteAsync<string>();

        // The final result is the output of the last step (CombinationStep)
        return result;
    }
}
