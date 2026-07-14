using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.BackgroundJobs;

namespace uTPro.Feature.JobMonitor.TestSite.Examples;

/// <summary>
/// Registers example jobs the standard Umbraco way so the Job Monitor dashboard has
/// something to display. No Job Monitor-specific wiring is required.
/// </summary>
public sealed class ExampleJobsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Modern recurring job — discovered and fully monitored.
        builder.Services.AddRecurringBackgroundJob<ExampleModernJob>();

        // Legacy recurring job — listed with limited support.
        builder.Services.AddHostedService<ExampleLegacyJob>();

        // Plain hosted service — must be excluded from the job list.
        builder.Services.AddHostedService<ExamplePlainHostedService>();
    }
}
