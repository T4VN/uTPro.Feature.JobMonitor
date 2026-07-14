using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Runs a recurring background job on demand, safely (off the request thread, overlap-guarded,
/// role-checked, and recorded as a manual execution).
/// </summary>
public interface IManualTriggerService
{
    /// <summary>
    /// Attempts to start <paramref name="jobKey"/> on a background thread.
    /// Returns <see cref="TriggerResult.Started"/> only if the run began; the acknowledgement
    /// is returned before the job completes.
    /// </summary>
    Task<TriggerResult> TryTriggerAsync(string jobKey, int userId);

    /// <summary><c>true</c> while a manual run of the given job is in progress.</summary>
    bool IsRunning(string jobKey);
}
