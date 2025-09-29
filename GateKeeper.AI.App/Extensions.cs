using GateKeeper.AI.Orchestrator;
using GateKeeper.AI.Shared;

namespace GateKeeper.AI.App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services,ConfigurationManager config)
    {

        services.AddSingleton<Settings>(_ => {
            var settings = new Settings(config);
            //settings.GitSettings = settings.GetSettings<Settings.GitHubSettings>();
            return settings;
        });
        services.AddScoped<IOrchestratorService, OrchestratorService>();
        services.AddOrchestrationServices();

        // Add Azure services here
        return services;
    }
}
