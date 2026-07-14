namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// A single execution record returned by the history endpoint.
/// </summary>
public sealed class ExecutionRecordViewModel
{
    public required string StartUtc { get; init; }
    public string? EndUtc { get; init; }
    public double? DurationSeconds { get; init; }
    public string? Outcome { get; init; }
    public required string Source { get; init; }
    public int? InitiatingUserId { get; init; }
    public bool IsRunning { get; init; }

    public static ExecutionRecordViewModel From(ExecutionRecord record) => new()
    {
        StartUtc = record.StartUtc.ToString("o"),
        EndUtc = record.EndUtc?.ToString("o"),
        DurationSeconds = record.Duration?.TotalSeconds,
        Outcome = record.Outcome?.ToString(),
        Source = record.Source.ToString(),
        InitiatingUserId = record.InitiatingUserId,
        IsRunning = !record.IsComplete
    };
}
