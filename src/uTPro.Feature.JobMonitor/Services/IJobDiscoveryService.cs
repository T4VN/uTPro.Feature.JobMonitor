using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Enumerates and classifies every recurring background job registered in the host,
/// excluding plain hosted services.
/// </summary>
public interface IJobDiscoveryService
{
    /// <summary>
    /// Returns one descriptor per registered recurring job. Keys are unique; plain hosted
    /// services are absent; ordering is stable across calls within a process. Never throws.
    /// </summary>
    IReadOnlyList<JobDescriptor> DiscoverJobs();

    /// <summary>Finds a single discovered job by its key, or <c>null</c>.</summary>
    JobDescriptor? Find(string jobKey);
}
