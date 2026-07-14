using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace uTPro.Feature.JobMonitor.Migrations;

/// <summary>
/// Creates the <c>uTProJobExecution</c> table used by the durable execution store.
/// The schema is built from an immutable snapshot so column types resolve per database
/// provider (SQL Server, SQLite, PostgreSQL).
/// </summary>
public class InitJobMonitor : AsyncMigrationBase
{
    public InitJobMonitor(IMigrationContext context) : base(context) { }

    protected override Task MigrateAsync()
    {
        if (!TableExists("uTProJobExecution"))
        {
            Create.Table<JobExecutionSchema>().Do();
        }

        return Task.CompletedTask;
    }

    // ── Immutable schema snapshot (do not change after release; add new steps for changes) ──

    [TableName("uTProJobExecution")]
    [PrimaryKey("id", AutoIncrement = true)]
    [ExplicitColumns]
    public class JobExecutionSchema
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        public int Id { get; set; }

        [Column("RunId")]
        [Length(36)]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_uTProJobExecution_RunId")]
        public string RunId { get; set; } = string.Empty;

        [Column("JobKey")]
        [Length(400)]
        [Index(IndexTypes.NonClustered, Name = "IX_uTProJobExecution_JobKey")]
        public string JobKey { get; set; } = string.Empty;

        [Column("StartUtc")]
        public DateTime StartUtc { get; set; }

        [Column("EndUtc")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? EndUtc { get; set; }

        [Column("DurationMs")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public long? DurationMs { get; set; }

        [Column("Outcome")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(20)]
        public string? Outcome { get; set; }

        [Column("Source")]
        [Length(20)]
        public string Source { get; set; } = string.Empty;

        [Column("InitiatingUserId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? InitiatingUserId { get; set; }
    }
}
