using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Captures and retains execution telemetry per job.
/// </summary>
public interface IExecutionTracker
{
    /// <summary>The storage mode of the backing store (surfaced to the dashboard).</summary>
    StorageMode StorageMode { get; }

    /// <summary>
    /// Records the start of an execution. The returned record is mutated in place when the
    /// execution completes via <see cref="RecordEnd"/>.
    /// </summary>
    ExecutionRecord RecordStart(string jobKey, TriggerSource source, int? initiatingUserId = null);

    /// <summary>Records the completion (outcome + end time + duration) of a previously started execution.</summary>
    void RecordEnd(ExecutionRecord record, ExecutionOutcome outcome);

    /// <summary>Returns the most recent record for the key, or <c>null</c> when none exists.</summary>
    ExecutionRecord? GetLatest(string jobKey);

    /// <summary>Returns recent records for the key, most recent first.</summary>
    IReadOnlyList<ExecutionRecord> GetHistory(string jobKey);
}
