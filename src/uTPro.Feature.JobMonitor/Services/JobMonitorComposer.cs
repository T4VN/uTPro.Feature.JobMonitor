using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using uTPro.Feature.JobMonitor.Migrations;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Registers all Job Monitor services with the standard Umbraco pattern.
/// No per-job wiring is required — jobs are auto-discovered from the host's registrations.
/// The telemetry store is chosen from configuration (<c>uTPro:Feature:JobMonitor:Storage</c>).
/// </summary>
internal sealed class JobMonitorComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var options = builder.Config.GetSection(JobMonitorOptions.SectionName).Get<JobMonitorOptions>()
                      ?? new JobMonitorOptions();

        builder.Services.Configure<JobMonitorOptions>(builder.Config.GetSection(JobMonitorOptions.SectionName));

        if (options.Storage == StorageMode.Durable)
        {
            builder.Services.AddSingleton<IExecutionStore, DurableExecutionStore>();

            // Only wire the schema migration when durable storage is actually enabled.
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, JobMonitorMigrationHandler>();
        }
        else
        {
            var capacity = options.EffectiveHistoryCapacity;
            builder.Services.AddSingleton<IExecutionStore>(_ => new InMemoryExecutionStore(capacity));
        }

        builder.Services.AddSingleton<IExecutionTracker, ExecutionTracker>();
        builder.Services.AddSingleton<ITimingReaderService, TimingReaderService>();
        builder.Services.AddSingleton<IJobDiscoveryService, JobDiscoveryService>();
        builder.Services.AddSingleton<INextRunEstimator, NextRunEstimator>();
        builder.Services.AddSingleton<IManualTriggerService, ManualTriggerService>();
    }
}
