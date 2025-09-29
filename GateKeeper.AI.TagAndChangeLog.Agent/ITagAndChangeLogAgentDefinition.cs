using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateKeeper.AI.TagAndChangeLog.Agent
{
    public interface ITagAndChangeLogAgentDefinition
    {
        Task<(Kernel, ChatCompletionAgent)> CreateAgent();
    }
}
