using System.Reflection;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.BackgroundJobs;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Reads timing parameters directly from modern jobs and via tolerant reflection from legacy jobs.
/// </summary>
public sealed class TimingReaderService : ITimingReaderService
{
    private const BindingFlags NonPublicInstance =
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    public TimingInfo ReadTiming(object job, JobModel model)
        => model == JobModel.Modern ? ReadModern(job) : ReadLegacy(job);

    private static TimingInfo ReadModern(object job)
    {
        if (job is not IRecurringBackgroundJob modern)
        {
            return TimingInfo.Empty;
        }

        return new TimingInfo
        {
            Period = TryRead(() => modern.Period),
            Delay = TryRead(() => modern.Delay),
            ServerRoles = TryRead(() => MapRoles(modern.ServerRoles))
        };
    }

    private static TimingInfo ReadLegacy(object job)
    {
        // RecurringHostedServiceBase keeps period/delay in private fields (_period / _delay).
        return new TimingInfo
        {
            Period = ReadTimeSpanField(job, "_period"),
            Delay = ReadTimeSpanField(job, "_delay"),
            ServerRoles = Available<NodeServerRole[]>.Unavailable
        };
    }

    private static Available<TimeSpan> ReadTimeSpanField(object job, string fieldName)
    {
        try
        {
            var field = job.GetType().GetField(fieldName, NonPublicInstance);
            if (field is not null && field.GetValue(job) is TimeSpan value)
            {
                return Available<TimeSpan>.Of(value);
            }
        }
        catch
        {
            // fall through to unavailable
        }

        return Available<TimeSpan>.Unavailable;
    }

    private static NodeServerRole[] MapRoles(ServerRole[]? roles)
        => (roles ?? Array.Empty<ServerRole>()).Select(ServerRoleMapper.ToNodeRole).ToArray();

    private static Available<T> TryRead<T>(Func<T> read)
    {
        try
        {
            return Available<T>.Of(read());
        }
        catch
        {
            return Available<T>.Unavailable;
        }
    }
}
