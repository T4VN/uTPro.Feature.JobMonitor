namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// The outcome of a completed job execution.
/// </summary>
public enum ExecutionOutcome
{
    /// <summary>The job completed without throwing.</summary>
    Success,

    /// <summary>The job threw an exception during execution.</summary>
    Failure
}
