using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Durable telemetry store backed by the Umbraco database (<c>uTProJobExecution</c> table).
/// History survives host restarts and is shared across load-balanced nodes. All access uses
/// NPoco strongly-typed queries so identifiers are quoted per provider (SQL Server, SQLite,
/// PostgreSQL). Failures degrade gracefully — telemetry never breaks a running job.
/// </summary>
public sealed class DurableExecutionStore(
    IScopeProvider scopeProvider,
    IOptions<JobMonitorOptions> options,
    ILogger<DurableExecutionStore> logger) : IExecutionStore
{
    private readonly int _capacity = options.Value.EffectiveHistoryCapacity;

    public StorageMode Mode => StorageMode.Durable;

    public void Add(ExecutionRecord record)
    {
        try
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            scope.Database.Insert(ToDto(record));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist execution start for job {JobKey}.", record.JobKey);
        }
    }

    public void Update(ExecutionRecord record)
    {
        try
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            var runId = record.RunId.ToString();
            var sql = scope.SqlContext.Sql()
                .SelectAll().From<JobExecutionDto>()
                .Where<JobExecutionDto>(x => x.RunId == runId);

            var dto = scope.Database.SingleOrDefault<JobExecutionDto>(sql);
            if (dto is null)
            {
                // No start row found (e.g. store switched mode) — insert the completed record instead.
                scope.Database.Insert(ToDto(record));
                return;
            }

            dto.EndUtc = record.EndUtc?.UtcDateTime;
            dto.DurationMs = record.Duration.HasValue ? (long)record.Duration.Value.TotalMilliseconds : null;
            dto.Outcome = record.Outcome?.ToString();
            scope.Database.Update(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist execution completion for job {JobKey}.", record.JobKey);
        }
    }

    public ExecutionRecord? GetLatest(string jobKey)
    {
        try
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            var sql = scope.SqlContext.Sql()
                .SelectAll().From<JobExecutionDto>()
                .Where<JobExecutionDto>(x => x.JobKey == jobKey)
                .OrderByDescending<JobExecutionDto>(x => x.StartUtc);

            var dto = scope.Database.Fetch<JobExecutionDto>(1, 1, sql).FirstOrDefault();
            return dto is null ? null : ToRecord(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read latest execution for job {JobKey}.", jobKey);
            return null;
        }
    }

    public IReadOnlyList<ExecutionRecord> GetHistory(string jobKey)
    {
        try
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            var sql = scope.SqlContext.Sql()
                .SelectAll().From<JobExecutionDto>()
                .Where<JobExecutionDto>(x => x.JobKey == jobKey)
                .OrderByDescending<JobExecutionDto>(x => x.StartUtc);

            return scope.Database.Fetch<JobExecutionDto>(1, _capacity, sql)
                .Select(ToRecord)
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read execution history for job {JobKey}.", jobKey);
            return Array.Empty<ExecutionRecord>();
        }
    }

    private static JobExecutionDto ToDto(ExecutionRecord record) => new()
    {
        RunId = record.RunId.ToString(),
        JobKey = record.JobKey,
        StartUtc = record.StartUtc.UtcDateTime,
        EndUtc = record.EndUtc?.UtcDateTime,
        DurationMs = record.Duration.HasValue ? (long)record.Duration.Value.TotalMilliseconds : null,
        Outcome = record.Outcome?.ToString(),
        Source = record.Source.ToString(),
        InitiatingUserId = record.InitiatingUserId
    };

    private static ExecutionRecord ToRecord(JobExecutionDto dto) => new()
    {
        RunId = Guid.TryParse(dto.RunId, out var id) ? id : Guid.Empty,
        JobKey = dto.JobKey,
        StartUtc = new DateTimeOffset(DateTime.SpecifyKind(dto.StartUtc, DateTimeKind.Utc)),
        EndUtc = dto.EndUtc.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(dto.EndUtc.Value, DateTimeKind.Utc)) : null,
        Duration = dto.DurationMs.HasValue ? TimeSpan.FromMilliseconds(dto.DurationMs.Value) : null,
        Outcome = Enum.TryParse<ExecutionOutcome>(dto.Outcome, out var outcome) ? outcome : null,
        Source = Enum.TryParse<TriggerSource>(dto.Source, out var source) ? source : TriggerSource.Scheduled,
        InitiatingUserId = dto.InitiatingUserId
    };
}
