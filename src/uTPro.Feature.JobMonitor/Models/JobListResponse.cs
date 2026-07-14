namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// The payload returned by <c>GET jobs</c>.
/// </summary>
public sealed class JobListResponse
{
    public required int Count { get; init; }

    /// <summary>"Unknown" | "Single" | "SchedulingPublisher" | "Subscriber".</summary>
    public required string ServerRole { get; init; }

    /// <summary><c>false</c> on a Subscriber / Unknown node (scheduled jobs do not run there).</summary>
    public required bool RoleExecutesJobs { get; init; }

    /// <summary>"InMemory" | "Durable".</summary>
    public required string StorageMode { get; init; }

    public required IReadOnlyList<JobViewModel> Jobs { get; init; }
}
