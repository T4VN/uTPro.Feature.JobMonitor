namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// How execution telemetry is retained.
/// </summary>
public enum StorageMode
{
    /// <summary>Records live in memory for the lifetime of the host process (reset on restart, per-node).</summary>
    InMemory,

    /// <summary>Records are persisted to a durable store that survives restarts.</summary>
    Durable
}
