using Microsoft.Extensions.Configuration;

namespace GateKeeper.AI.Shared;

public class Settings
{
    private readonly IConfiguration configRoot;

    public Settings(IConfiguration config)
    {
        configRoot = config;
    }

    private AzureOpenAISettings azureOpenAI;
    private OpenAISettings openAI;

    public AzureOpenAISettings AzureOpenAI => this.azureOpenAI ??= this.GetSettings<Settings.AzureOpenAISettings>();

    public OpenAISettings OpenAI => this.openAI ??= this.GetSettings<Settings.OpenAISettings>();

    public GitHubSettings GitSettings => this.GetSettings<GitHubSettings>();

    public FoundrySettings Foundry => this.GetSettings<FoundrySettings>();

    public class OpenAISettings
    {
        public string ChatModel { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAISettings
    {
        public string ChatModelDeployment { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class FoundrySettings
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
    }

    public class GitHubSettings
    {
        public string BaseUrl { get; set; } = "https://api.github.com";
        public string Token { get; set; } = string.Empty;

        public string Owner { get; set; } = "AIS-IDC-Hackathon-2025";
        public string Repo { get; set; } = "copilot_security";

    }
    public TSettings GetSettings<TSettings>() =>
        this.configRoot.GetRequiredSection(typeof(TSettings).Name).Get<TSettings>()!;
}


