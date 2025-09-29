using Azure.AI.Agents.Persistent;
using GateKeeper.AI.Shared;
using GateKeeper.AI.Shared.MCP;
using GateKeeper.AI.TagAndChangeLog.Agent.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static GateKeeper.AI.Shared.Settings;

namespace GateKeeper.AI.TagAndChangeLog.Agent
{
    public class TagAndChangeLogAgentDefinition(Settings settings, ILoggerFactory loggerFactory) : ITagAndChangeLogAgentDefinition
    {
        public async Task<(Kernel, ChatCompletionAgent)> CreateAgent()
        {
            // GitHubSettings githubSettings = settings.GetSettings<GitHubSettings>();
            GitHubPlugin githubPlugin = new(settings);

            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: settings.AzureOpenAI.ChatModelDeployment,
                endpoint: settings.AzureOpenAI.Endpoint,
                apiKey: settings.AzureOpenAI.ApiKey);

            builder.Plugins.AddFromObject(githubPlugin);

            Kernel tagAgentKernel = builder.Build();

            ChatCompletionAgent agent = new()
            {
                Name = "Tag_And_ChangeLog_Agent",
                Instructions =
                        """
                        You are an agent designed to query and retrieve information from a single GitHub repository.
                        You can perform read operations (get user profile, repository info, issues, commits) and also create tags.
                        You are also able to access the profile of the active user.

                        Use the current date and time to provide up-to-date details or time-sensitive responses.

                        The repository you are querying is a public repository with the following details:
                        Base URL GitHub owner: https://github.com/AIS-IDC-Hackathon-2025/
                        Organization: {githubSettings.Owner}
                        Repository: {githubSettings.Repo}

                        Available operations:
                        - Get user profile
                        - Get repository information
                        - Get repository issues
                        - Get issue details
                        - Get latest commit from a branch
                        - Create tags on the latest commit

                        The current date and time is: {{$now}}. 
                        """.Replace("{githubSettings.Owner}", settings.GitSettings.Owner).Replace("{githubSettings.Repo}", settings.GitSettings.Repo),
                Kernel = tagAgentKernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "$now", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                    }
            };

            return (tagAgentKernel, agent);



        }
    }
}
