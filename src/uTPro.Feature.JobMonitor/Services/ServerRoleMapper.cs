using Umbraco.Cms.Core.Sync;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Maps between Umbraco's <see cref="ServerRole"/> and the package's stable <see cref="NodeServerRole"/>.
/// </summary>
internal static class ServerRoleMapper
{
    public static NodeServerRole ToNodeRole(ServerRole role) => role switch
    {
        ServerRole.Single => NodeServerRole.Single,
        ServerRole.SchedulingPublisher => NodeServerRole.SchedulingPublisher,
        ServerRole.Subscriber => NodeServerRole.Subscriber,
        _ => NodeServerRole.Unknown
    };

    /// <summary>
    /// Recurring jobs execute only on a Single node or the SchedulingPublisher, mirroring
    /// Umbraco's own guard in <c>RecurringBackgroundJobHostedService</c>.
    /// </summary>
    public static bool NodeExecutesJobs(ServerRole role)
        => role is ServerRole.Single or ServerRole.SchedulingPublisher;

    public static bool NodeExecutesJobs(NodeServerRole role)
        => role is NodeServerRole.Single or NodeServerRole.SchedulingPublisher;
}
