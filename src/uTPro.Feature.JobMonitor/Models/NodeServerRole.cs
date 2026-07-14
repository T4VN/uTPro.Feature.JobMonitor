namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Mirrors Umbraco's <c>ServerRole</c> so the package exposes a stable API surface
/// that is independent of the Umbraco enum's assembly.
/// </summary>
public enum NodeServerRole
{
    Unknown,
    Single,
    SchedulingPublisher,
    Subscriber
}
