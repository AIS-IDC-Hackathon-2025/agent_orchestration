using GateKeeper.AI.Shared;
using GateKeeper.AI.Shared.MCP;
using GateKeeper.AI.SmartCodeReviewer.Agent;
using GateKeeper.AI.TagAndChangeLog.Agent;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateKeeper.AI.Orchestrator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrchestrationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMcpClientHost, McpClientHost>(options => {
            McpClientHost host = new McpClientHost(options.GetRequiredService<Settings>().GitSettings.Token);
            return host;
        });
        services.AddSingleton<ITagAndChangeLogAgentDefinition, TagAndChangeLogAgentDefinition>();
        services.AddSingleton<ISmartCodeReviewerAgentDefinition, SmartCodeReviewerAgentDefinition>();

        // Add Azure services here
        return services;
    }
}