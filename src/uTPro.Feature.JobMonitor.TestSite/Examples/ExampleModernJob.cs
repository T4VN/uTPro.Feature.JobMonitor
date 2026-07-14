using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.BackgroundJobs;

namespace uTPro.Feature.JobMonitor.TestSite.Examples;

/// <summary>
/// A standard modern recurring job. Job Monitor discovers it automatically and offers
/// full monitoring (timing, telemetry, "Run now").
/// </summary>
public sealed class ExampleModernJob(ILogger<ExampleModernJob> logger) : IRecurringBackgroundJob
{
    public TimeSpan Period => TimeSpan.FromMinutes(5);

    public TimeSpan Delay => TimeSpan.FromSeconds(30);

    public ServerRole[] ServerRoles => new[] { ServerRole.Single, ServerRole.SchedulingPublisher };

    public event EventHandler? PeriodChanged { add { } remove { } }

    public Task RunJobAsync()
    {
        logger.LogInformation("ExampleModernJob ran at {Time}.", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}
