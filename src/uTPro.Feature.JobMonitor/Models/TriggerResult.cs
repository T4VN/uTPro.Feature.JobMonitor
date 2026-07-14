namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Result of a manual-trigger ("Run now") attempt.
/// </summary>
public enum TriggerResult
{
    /// <summary>The run was accepted and started on a background thread.</summary>
    Started,

    /// <summary>The job is already executing (scheduled or manual); the new run was declined.</summary>
    AlreadyRunning,

    /// <summary>The current node's server role does not permit the job to execute.</summary>
    RoleNotPermitted,

    /// <summary>No triggerable job matched the supplied key.</summary>
    NotFound
}
