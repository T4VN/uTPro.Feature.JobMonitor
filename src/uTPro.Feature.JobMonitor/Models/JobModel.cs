namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Classification of a discovered recurring background job.
/// </summary>
public enum JobModel
{
    /// <summary>Implements Umbraco's <c>IRecurringBackgroundJob</c> interface (full monitoring support).</summary>
    Modern,

    /// <summary>Derives from Umbraco's <c>RecurringHostedServiceBase</c> (limited monitoring support).</summary>
    Legacy
}
