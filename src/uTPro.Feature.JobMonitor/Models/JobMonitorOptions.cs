namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Configuration for Job Monitor, bound from the <c>uTPro:Feature:JobMonitor</c> section.
/// </summary>
public sealed class JobMonitorOptions
{
    public const string SectionName = "uTPro:Feature:JobMonitor";

    /// <summary>
    /// Where execution telemetry is retained. <see cref="StorageMode.InMemory"/> (default) keeps
    /// records in a per-process ring buffer; <see cref="StorageMode.Durable"/> persists them to the
    /// Umbraco database (survives restarts and is shared across load-balanced nodes).
    /// </summary>
    public StorageMode Storage { get; set; } = StorageMode.InMemory;

    /// <summary>
    /// Maximum number of execution records retained per job (ring-buffer size for in-memory,
    /// query cap for durable history). Values &lt;= 0 fall back to the default of 50.
    /// </summary>
    public int HistoryCapacity { get; set; } = 50;

    /// <summary>
    /// How long (seconds) a discovery result is cached before jobs are re-enumerated.
    /// 0 disables caching (always fresh). Defaults to 30.
    /// </summary>
    public int DiscoveryCacheSeconds { get; set; } = 30;

    /// <summary>Normalized capacity (never &lt;= 0).</summary>
    public int EffectiveHistoryCapacity => HistoryCapacity > 0 ? HistoryCapacity : 50;

    /// <summary>Normalized cache duration (never negative).</summary>
    public TimeSpan DiscoveryCacheDuration
        => TimeSpan.FromSeconds(DiscoveryCacheSeconds > 0 ? DiscoveryCacheSeconds : 0);
}
