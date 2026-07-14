namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// A single recorded execution of a recurring background job. Immutable: a running record is
/// created by the tracker and, on completion, replaced by a new record via <see cref="Complete"/>.
/// This means readers always observe a fully-constructed snapshot (no torn reads).
/// </summary>
/// <remarks>
/// Invariants:
/// <list type="bullet">
/// <item><description><see cref="EndUtc"/> &gt;= <see cref="StartUtc"/> when set.</description></item>
/// <item><description><see cref="Duration"/> == <see cref="EndUtc"/> - <see cref="StartUtc"/> when both set.</description></item>
/// <item><description><see cref="Outcome"/>, <see cref="EndUtc"/> and <see cref="Duration"/> are set together at completion.</description></item>
/// <item><description><see cref="Source"/> == <see cref="TriggerSource.Manual"/> requires <see cref="InitiatingUserId"/> to be set.</description></item>
/// </list>
/// </remarks>
public sealed class ExecutionRecord
{
    /// <summary>Stable id for this run, used to correlate the running and completed records.</summary>
    public required Guid RunId { get; init; }

    public required string JobKey { get; init; }
    public required DateTimeOffset StartUtc { get; init; }
    public DateTimeOffset? EndUtc { get; init; }
    public TimeSpan? Duration { get; init; }

    /// <summary><c>null</c> while the execution is still running.</summary>
    public ExecutionOutcome? Outcome { get; init; }

    public required TriggerSource Source { get; init; }

    /// <summary>Set for manual runs (the backoffice user id that initiated the run).</summary>
    public int? InitiatingUserId { get; init; }

    /// <summary><c>true</c> once <see cref="Outcome"/> has been recorded.</summary>
    public bool IsComplete => Outcome.HasValue;

    /// <summary>Returns a completed copy of this (running) record.</summary>
    public ExecutionRecord Complete(DateTimeOffset endUtc, ExecutionOutcome outcome)
    {
        if (endUtc < StartUtc)
        {
            endUtc = StartUtc;
        }

        return new ExecutionRecord
        {
            RunId = RunId,
            JobKey = JobKey,
            StartUtc = StartUtc,
            Source = Source,
            InitiatingUserId = InitiatingUserId,
            EndUtc = endUtc,
            Duration = endUtc - StartUtc,
            Outcome = outcome
        };
    }
}
