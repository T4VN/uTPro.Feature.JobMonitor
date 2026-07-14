using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Stores execution telemetry for recurring background jobs. Implementations must retain
/// at least the most recent record per job key.
/// </summary>
public interface IExecutionStore
{
    /// <summary>How this store retains records.</summary>
    StorageMode Mode { get; }

    /// <summary>Adds a new (running) record for a job.</summary>
    void Add(ExecutionRecord record);

    /// <summary>Replaces an existing record (matched by <see cref="ExecutionRecord.RunId"/>) with its completed form.</summary>
    void Update(ExecutionRecord record);

    /// <summary>Returns the most recent record for the key, or <c>null</c> when none exists.</summary>
    ExecutionRecord? GetLatest(string jobKey);

    /// <summary>Returns recent records for the key, most recent first (bounded).</summary>
    IReadOnlyList<ExecutionRecord> GetHistory(string jobKey);
}
