using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Default <see cref="IExecutionTracker"/> that writes through an <see cref="IExecutionStore"/>.
/// </summary>
public sealed class ExecutionTracker(IExecutionStore store) : IExecutionTracker
{
    private readonly TimeProvider _time = TimeProvider.System;

    public StorageMode StorageMode => store.Mode;

    public ExecutionRecord RecordStart(string jobKey, TriggerSource source, int? initiatingUserId = null)
    {
        if (source == TriggerSource.Manual && initiatingUserId is null)
        {
            throw new ArgumentException("Manual executions require an initiating user id.", nameof(initiatingUserId));
        }

        var record = new ExecutionRecord
        {
            RunId = Guid.NewGuid(),
            JobKey = jobKey,
            StartUtc = _time.GetUtcNow(),
            Source = source,
            InitiatingUserId = initiatingUserId
        };

        // Added while still running so an in-progress run is visible on the dashboard.
        store.Add(record);
        return record;
    }

    public void RecordEnd(ExecutionRecord record, ExecutionOutcome outcome)
    {
        var completed = record.Complete(_time.GetUtcNow(), outcome);
        store.Update(completed);
    }

    public ExecutionRecord? GetLatest(string jobKey) => store.GetLatest(jobKey);

    public IReadOnlyList<ExecutionRecord> GetHistory(string jobKey) => store.GetHistory(jobKey);
}
