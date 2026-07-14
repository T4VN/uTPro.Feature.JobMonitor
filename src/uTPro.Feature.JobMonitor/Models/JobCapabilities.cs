namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// The monitoring capabilities available for a given job. Every discovered job
/// always includes <see cref="Listing"/>.
/// </summary>
[Flags]
public enum JobCapabilities
{
    None = 0,
    Listing = 1,
    Timing = 2,
    Trigger = 4,
    Tracking = 8
}
