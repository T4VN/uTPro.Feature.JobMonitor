namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Identifies what initiated a job execution.
/// </summary>
public enum TriggerSource
{
    /// <summary>The execution was started by Umbraco's scheduler.</summary>
    Scheduled,

    /// <summary>The execution was started on demand from the backoffice ("Run now").</summary>
    Manual
}
