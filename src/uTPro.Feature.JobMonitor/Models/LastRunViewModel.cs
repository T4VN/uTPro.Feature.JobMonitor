namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// The most recent execution of a job, as shown on the dashboard.
/// </summary>
public sealed class LastRunViewModel
{
    /// <summary>ISO-8601 UTC start timestamp.</summary>
    public required string StartUtc { get; init; }

    /// <summary>Duration in seconds; <c>null</c> while still running.</summary>
    public double? DurationSeconds { get; init; }

    /// <summary>"Success" | "Failure"; <c>null</c> while still running.</summary>
    public string? Outcome { get; init; }

    /// <summary>"Scheduled" | "Manual".</summary>
    public required string Source { get; init; }

    public bool IsRunning { get; init; }
}
