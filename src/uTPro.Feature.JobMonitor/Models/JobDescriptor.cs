namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Server-side result of discovering a single recurring background job.
/// </summary>
/// <remarks>
/// Invariants:
/// <list type="bullet">
/// <item><description><see cref="JobKey"/> is non-empty and unique within a discovery pass.</description></item>
/// <item><description><see cref="Capabilities"/> always includes <see cref="JobCapabilities.Listing"/>.</description></item>
/// <item><description><see cref="CanTrigger"/> implies <see cref="Model"/> == <see cref="JobModel.Modern"/>.</description></item>
/// </list>
/// </remarks>
public sealed class JobDescriptor
{
    /// <summary>Stable identifier = full type name.</summary>
    public required string JobKey { get; init; }

    /// <summary>Display name (full type name).</summary>
    public required string TypeName { get; init; }

    public required JobModel Model { get; init; }

    public Available<TimeSpan> Period { get; init; } = Available<TimeSpan>.Unavailable;
    public Available<TimeSpan> Delay { get; init; } = Available<TimeSpan>.Unavailable;
    public Available<NodeServerRole[]> ServerRoles { get; init; } = Available<NodeServerRole[]>.Unavailable;

    /// <summary><c>true</c> only for modern, invocable jobs.</summary>
    public bool CanTrigger { get; init; }

    public JobCapabilities Capabilities { get; init; } = JobCapabilities.Listing;

    /// <summary>The live job instance (used for manual triggering). Not serialized.</summary>
    public object? Instance { get; init; }
}
