using Microsoft.AspNetCore.Components;

namespace GateKeeper.AI.App.Components.Pages;

public partial class Agents : ComponentBase
{
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
        new("Smar code review"),
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
            await Task.Delay(TimeSpan.FromSeconds(2.5), _cts.Token);

            step.Status = StepStatus.Completed;
            StatusMessage = $"{step.Name} completed successfully.";

            if (CurrentIndex < Steps.Count - 1)
                CurrentIndex++;
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
        StatusMessage = "Pipeline reset.";
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
