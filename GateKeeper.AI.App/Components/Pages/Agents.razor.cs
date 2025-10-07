using GateKeeper.AI.Orchestrator;
using Microsoft.AspNetCore.Components;

namespace GateKeeper.AI.App.Components.Pages;

public partial class Agents : ComponentBase
{
    private string? tagName;
    private string? prompt;

    [Inject]
    private IOrchestratorService OrchestratorService { get; set; } = default!;

    private enum StepStatus { Pending, InProgress, Completed, Failed }

    private sealed class PipelineStep
    {
        public string Name { get; }
        public StepStatus Status { get; set; } = StepStatus.Pending;
        public PipelineStep(string name) => Name = name;
    }

    private readonly List<PipelineStep> Steps =
    [
        new("Initialize"),
        new("Tagging and Change log"),
        new("Trust"),
        new("Smart code review"),
    ];

    private int CurrentIndex { get; set; } = 0;

    private CancellationTokenSource? _cts;

    private bool IsBusy => CurrentIndex < Steps.Count && Steps[CurrentIndex].Status == StepStatus.InProgress;

    private string? StatusMessage;

    private int CompletedCount => Steps.Count(s => s.Status == StepStatus.Completed);

    private double ProgressPercent => (double)CompletedCount / Steps.Count * 100d;

    private bool AllDone => CompletedCount == Steps.Count;

    private bool CanClickStart =>
        !IsBusy &&
        !AllDone &&
        (CurrentIndex < Steps.Count &&
         Steps[CurrentIndex].Status is StepStatus.Pending);

    private string StartButtonText =>
        AllDone ? "Done" :
        IsBusy ? "Running..." :
        "Start";

    private string CssFor(PipelineStep step, bool isActive) =>
        step.Status switch
        {
            StepStatus.Pending => isActive ? "pending active" : "pending",
            StepStatus.InProgress => "in-progress",
            StepStatus.Completed => "completed",
            StepStatus.Failed => "failed",
            _ => ""
        };

    private async Task StartAsync()
    {
        if(string.IsNullOrWhiteSpace(tagName))
        {
            StatusMessage = "Please enter a valid tag name.";
            return;
        }

        if (AllDone || IsBusy)
            return;

        var step = Steps[CurrentIndex];

        if (step.Status != StepStatus.Pending)
            return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        step.Status = StepStatus.InProgress;
        StatusMessage = $"Starting {step.Name}...";
        StateHasChanged();

        try
        {
            switch (step.Name)
            {
                case "Initialize":
                    OrchestratorService.InitializeKernels();
                    break;
                case "Tagging and Change log":
                    string tagAndChangeLogAgentMessage = $"create tag v{tagName} and generate release notes and push it to main on repo copilot_security";
                    await OrchestratorService.RunTaggingAndChangeLogAsync(tagAndChangeLogAgentMessage);
                    break;
                case "Trust":
                    string trustAgentMessage = $"Get vulnerabilities and license risk scanning report for the tags v1.2.509 and v{tagName} and then push the generated report to `main` branch on repo `copilot_security` under `Releases` folder";
                    await OrchestratorService.RunTrustAgentAsync(trustAgentMessage);
                    break;
                case "Smart code review":
                    string smartCRAgentMessage = $"Do a code review for the tag v{tagName}";
                    await OrchestratorService.RunSmartCodeReviewAsync(smartCRAgentMessage);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown step: {step.Name}");
            }

            step.Status = StepStatus.Completed;
            StatusMessage = $"{step.Name} completed successfully.";

            if (CurrentIndex < Steps.Count - 1)
            {
                CurrentIndex++;
                await Task.Delay(2000); // Small delay for better UX
                await StartAsync();
            }
            else
                StatusMessage = "Pipeline finished.";
        }
        catch (TaskCanceledException)
        {
            StatusMessage = $"{step.Name} canceled.";
            step.Status = StepStatus.Pending;
        }
        catch (Exception ex)
        {
            step.Status = StepStatus.Failed;
            StatusMessage = $"{step.Name} failed: {ex.Message}";
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private void Reset()
    {
        if (IsBusy)
            return;

        _cts?.Cancel();

        foreach (var s in Steps)
            s.Status = StepStatus.Pending;

        CurrentIndex = 0;
        tagName = null;
        StatusMessage = "Pipeline reset.";
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
