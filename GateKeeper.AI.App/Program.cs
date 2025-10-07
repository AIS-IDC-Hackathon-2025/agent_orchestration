using GateKeeper.AI.App;
using GateKeeper.AI.App.Components;
using GateKeeper.AI.Shared.Hub;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("TelemetryGKOrchestration");

// Enable model diagnostics with sensitive data.
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddConsoleExporter()
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddConsoleExporter();
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddServices(builder.Configuration);

// SignalR with Azure SignalR Service
var signalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"] 
    ?? Environment.GetEnvironmentVariable("AZURE_SIGNALR_CONNECTIONSTRING");

if (string.IsNullOrEmpty(signalRConnectionString))
{
    throw new InvalidOperationException("Azure SignalR connection string not found. Please provide it in configuration or environment variable AZURE_SIGNALR_CONNECTIONSTRING.");
}

builder
    .Services.AddSignalR(o => { o.EnableDetailedErrors = true; })
    .AddAzureSignalR(signalRConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<AgentsHub>("/agentshub");

app.Run();
