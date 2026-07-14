using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Computes the estimated next run of a job as <c>lastStart + Period</c>.
/// </summary>
public interface INextRunEstimator
{
    /// <summary>
    /// Returns the estimate when both a latest record and a readable period exist; otherwise unavailable.
    /// </summary>
    Available<DateTimeOffset> Estimate(string jobKey, Available<TimeSpan> period);
}
