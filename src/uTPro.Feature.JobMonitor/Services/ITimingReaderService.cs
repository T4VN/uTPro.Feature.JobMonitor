using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Extracts timing parameters from a job instance, tolerant of missing / inaccessible values.
/// </summary>
public interface ITimingReaderService
{
    /// <summary>
    /// Reads timing for a discovered job. Never throws: each field is available-with-value
    /// or explicitly unavailable.
    /// </summary>
    TimingInfo ReadTiming(object job, JobModel model);
}
