using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;

namespace GateKeeper.AI.UI.Services;

public interface AgentSetup
{
    public static ChatCompletionAgent CreatePhysicsAgent(Kernel kernel) =>
        new ChatCompletionAgent
        {
            Name = "PhysicsExpert",
            Instructions = "You are an expert in physics. Answer physics questions.",
            Kernel = kernel,
        };

    public static ChatCompletionAgent CreateChemistryAgent(Kernel kernel) =>
        new ChatCompletionAgent
        {
            Name = "ChemistryExpert",
            Instructions = "You are an expert in chemistry. Answer chemistry questions.",
            Kernel = kernel,
        };
}
