using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Estimates the next run as the most recent run's start time plus the job's period.
/// </summary>
public sealed class NextRunEstimator(IExecutionTracker tracker) : INextRunEstimator
{
    public Available<DateTimeOffset> Estimate(string jobKey, Available<TimeSpan> period)
    {
        var latest = tracker.GetLatest(jobKey);
        if (latest is null || !period.IsAvailable)
        {
            return Available<DateTimeOffset>.Unavailable;
        }

        return Available<DateTimeOffset>.Of(latest.StartUtc + period.Value);
    }
}
