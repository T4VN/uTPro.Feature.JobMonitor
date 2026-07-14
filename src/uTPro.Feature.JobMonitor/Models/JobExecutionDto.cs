using NPoco;

namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// NPoco runtime DTO for the durable execution-telemetry table. Timestamps are stored as UTC
/// <see cref="DateTime"/> and durations as milliseconds so the shape is portable across
/// SQL Server, SQLite and PostgreSQL (which have inconsistent DateTimeOffset support).
/// </summary>
[TableName("uTProJobExecution")]
[PrimaryKey("id", AutoIncrement = true)]
public class JobExecutionDto
{
    [Column("id")]
    public int Id { get; set; }

    [Column("RunId")]
    public string RunId { get; set; } = string.Empty;

    [Column("JobKey")]
    public string JobKey { get; set; } = string.Empty;

    [Column("StartUtc")]
    public DateTime StartUtc { get; set; }

    [Column("EndUtc")]
    public DateTime? EndUtc { get; set; }

    [Column("DurationMs")]
    public long? DurationMs { get; set; }

    [Column("Outcome")]
    public string? Outcome { get; set; }

    [Column("Source")]
    public string Source { get; set; } = string.Empty;

    [Column("InitiatingUserId")]
    public int? InitiatingUserId { get; set; }
}
