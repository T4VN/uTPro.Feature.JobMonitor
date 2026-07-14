using Umbraco.Cms.Infrastructure.HostedServices;

namespace uTPro.Feature.JobMonitor.TestSite.Examples;

// This example deliberately uses the legacy RecurringHostedServiceBase API (obsolete in
// Umbraco 18, scheduled for removal in v19) to exercise Job Monitor's "Legacy_Job" path.
// The obsolete warnings are intentional here and suppressed for a clean build.
#pragma warning disable CS0618 // obsolete constructor
#pragma warning disable CS0672 // overrides obsolete member

/// <summary>
/// A legacy recurring job deriving from <see cref="RecurringHostedServiceBase"/>.
/// Job Monitor lists it with limited support (timing via reflection; no "Run now").
/// </summary>
public sealed class ExampleLegacyJob(ILogger<ExampleLegacyJob> logger)
    : RecurringHostedServiceBase(logger, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1))
{
    public override Task PerformExecuteAsync(object? state)
    {
        logger.LogInformation("ExampleLegacyJob ran at {Time}.", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}

#pragma warning restore CS0672
#pragma warning restore CS0618
