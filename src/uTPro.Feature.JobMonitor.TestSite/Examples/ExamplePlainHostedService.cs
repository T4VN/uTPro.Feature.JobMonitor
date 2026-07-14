namespace uTPro.Feature.JobMonitor.TestSite.Examples;

/// <summary>
/// A plain hosted service that is NOT a recurring job. Job Monitor must exclude it from the list.
/// </summary>
public sealed class ExamplePlainHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
