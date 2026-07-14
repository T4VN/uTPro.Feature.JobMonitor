namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Timing parameters extracted from a job. Every field is either available-with-value
/// or explicitly unavailable; reading never throws.
/// </summary>
public sealed class TimingInfo
{
    public Available<TimeSpan> Period { get; init; } = Available<TimeSpan>.Unavailable;
    public Available<TimeSpan> Delay { get; init; } = Available<TimeSpan>.Unavailable;
    public Available<NodeServerRole[]> ServerRoles { get; init; } = Available<NodeServerRole[]>.Unavailable;

    /// <summary><c>true</c> when at least one timing field could be read.</summary>
    public bool AnyAvailable => Period.IsAvailable || Delay.IsAvailable || ServerRoles.IsAvailable;

    public static TimingInfo Empty => new();
}
