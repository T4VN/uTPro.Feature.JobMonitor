namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// A single job as presented to the backoffice dashboard.
/// </summary>
public sealed class JobViewModel
{
    public required string Key { get; init; }
    public required string TypeName { get; init; }

    /// <summary>"Modern" | "Legacy".</summary>
    public required string Model { get; init; }

    /// <summary><c>true</c> for legacy jobs (limited monitoring support).</summary>
    public required bool LimitedSupport { get; init; }

    public required bool CanTrigger { get; init; }

    public TimingViewModel Timing { get; init; } = new();

    /// <summary><c>null</c> = not yet recorded.</summary>
    public LastRunViewModel? LastRun { get; init; }

    /// <summary>ISO-8601 UTC estimate; <c>null</c> = unavailable.</summary>
    public string? EstimatedNextRunUtc { get; init; }

    public required string[] Capabilities { get; init; }
}
