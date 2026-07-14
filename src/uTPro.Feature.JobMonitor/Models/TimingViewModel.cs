namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Client-facing timing values. Each field is <c>null</c> when the underlying value is unavailable.
/// Durations are serialized as total seconds (the frontend renders them human-readable).
/// </summary>
public sealed class TimingViewModel
{
    public double? PeriodSeconds { get; init; }
    public double? DelaySeconds { get; init; }
    public string[]? ServerRoles { get; init; }

    public bool PeriodAvailable { get; init; }
    public bool DelayAvailable { get; init; }
    public bool ServerRolesAvailable { get; init; }
}
