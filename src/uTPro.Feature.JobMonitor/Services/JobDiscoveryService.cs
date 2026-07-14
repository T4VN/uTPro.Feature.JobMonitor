using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Infrastructure.BackgroundJobs;
using Umbraco.Cms.Infrastructure.HostedServices;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Discovers recurring background jobs from the host's registered services.
/// Modern jobs are unwrapped from their <see cref="RecurringBackgroundJobHostedService{TJob}"/>
/// wrappers (and supplemented by any directly-registered <see cref="IRecurringBackgroundJob"/>);
/// legacy jobs are the <see cref="RecurringHostedServiceBase"/> derivations that are not wrappers.
/// Plain hosted services are excluded.
/// Results are cached for a short, configurable window to avoid re-enumerating DI on every request.
/// </summary>
public sealed class JobDiscoveryService(
    IServiceProvider serviceProvider,
    ITimingReaderService timingReader,
    IOptions<JobMonitorOptions> options,
    ILogger<JobDiscoveryService> logger) : IJobDiscoveryService
{
    private const BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private readonly TimeSpan _cacheDuration = options.Value.DiscoveryCacheDuration;
    private readonly Lock _cacheGate = new();
    private IReadOnlyList<JobDescriptor>? _cache;
    private DateTime _cacheExpiresUtc;

    public IReadOnlyList<JobDescriptor> DiscoverJobs()
    {
        if (_cacheDuration <= TimeSpan.Zero)
        {
            return Enumerate();
        }

        lock (_cacheGate)
        {
            if (_cache is not null && DateTime.UtcNow < _cacheExpiresUtc)
            {
                return _cache;
            }

            _cache = Enumerate();
            _cacheExpiresUtc = DateTime.UtcNow.Add(_cacheDuration);
            return _cache;
        }
    }

    private IReadOnlyList<JobDescriptor> Enumerate()
    {
        var result = new List<JobDescriptor>();
        var seenKeys = new HashSet<string>(StringComparer.Ordinal);

        try
        {
            var hostedServices = serviceProvider.GetServices<IHostedService>().ToList();

            // 1. Modern jobs: unwrap RecurringBackgroundJobHostedService<T> to reach the live job.
            foreach (var hosted in hostedServices)
            {
                var job = TryUnwrapModernJob(hosted);
                if (job is null)
                {
                    continue;
                }

                AddModern(result, seenKeys, job);
            }

            // Supplement with any IRecurringBackgroundJob registered directly in DI.
            foreach (var job in serviceProvider.GetServices<IRecurringBackgroundJob>())
            {
                AddModern(result, seenKeys, job);
            }

            // 2. Legacy jobs: RecurringHostedServiceBase derivations that are not modern-job wrappers.
            foreach (var hosted in hostedServices)
            {
                if (hosted is RecurringHostedServiceBase && !IsModernWrapper(hosted.GetType()))
                {
                    AddLegacy(result, seenKeys, hosted);
                }
                // plain IHostedService that is not a recurring job → excluded (skipped)
            }
        }
        catch (Exception ex)
        {
            // Discovery must never take the host down; surface what we have.
            logger.LogError(ex, "Job Monitor discovery failed; returning partial results.");
        }

        return result
            .OrderBy(d => d.TypeName, StringComparer.Ordinal)
            .ToList();
    }

    public JobDescriptor? Find(string jobKey)
        => DiscoverJobs().FirstOrDefault(d => string.Equals(d.JobKey, jobKey, StringComparison.Ordinal));

    private void AddModern(List<JobDescriptor> result, HashSet<string> seenKeys, IRecurringBackgroundJob job)
    {
        var key = KeyOf(job);
        if (!seenKeys.Add(key))
        {
            return;
        }

        var timing = timingReader.ReadTiming(job, JobModel.Modern);
        result.Add(new JobDescriptor
        {
            JobKey = key,
            TypeName = key,
            Model = JobModel.Modern,
            Period = timing.Period,
            Delay = timing.Delay,
            ServerRoles = timing.ServerRoles,
            CanTrigger = true,
            Capabilities = JobCapabilities.Listing | JobCapabilities.Timing | JobCapabilities.Trigger | JobCapabilities.Tracking,
            Instance = job
        });
    }

    private void AddLegacy(List<JobDescriptor> result, HashSet<string> seenKeys, object job)
    {
        var key = KeyOf(job);
        if (!seenKeys.Add(key))
        {
            return;
        }

        var timing = timingReader.ReadTiming(job, JobModel.Legacy);
        var capabilities = JobCapabilities.Listing;
        if (timing.AnyAvailable)
        {
            capabilities |= JobCapabilities.Timing;
        }

        result.Add(new JobDescriptor
        {
            JobKey = key,
            TypeName = key,
            Model = JobModel.Legacy,
            Period = timing.Period,
            Delay = timing.Delay,
            ServerRoles = timing.ServerRoles,
            CanTrigger = false,
            Capabilities = capabilities,
            Instance = job
        });
    }

    /// <summary>
    /// Returns the inner <see cref="IRecurringBackgroundJob"/> if <paramref name="hosted"/> is a
    /// <see cref="RecurringBackgroundJobHostedService{TJob}"/> wrapper; otherwise <c>null</c>.
    /// </summary>
    private static IRecurringBackgroundJob? TryUnwrapModernJob(IHostedService hosted)
    {
        var type = hosted.GetType();
        if (!IsModernWrapper(type))
        {
            return null;
        }

        // The wrapper stores the job in a private "_job" field.
        var field = type.GetField("_job", NonPublicInstance);
        if (field?.GetValue(hosted) is IRecurringBackgroundJob job)
        {
            return job;
        }

        return null;
    }

    private static bool IsModernWrapper(Type type)
    {
        for (var t = type; t is not null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(RecurringBackgroundJobHostedService<>))
            {
                return true;
            }
        }

        return false;
    }

    private static string KeyOf(object instance) => instance.GetType().FullName ?? instance.GetType().Name;
}
