using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.BackgroundJobs;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// Executes jobs on demand with a non-blocking per-job gate so at most one manual run of a
/// given job is in flight at a time. Runs off the request thread and records the run as manual.
/// </summary>
public sealed class ManualTriggerService(
    IJobDiscoveryService discovery,
    IServerRoleAccessor serverRoleAccessor,
    IExecutionTracker tracker,
    ILogger<ManualTriggerService> logger) : IManualTriggerService
{
    // One gate per job key; TryEnter(0) makes acquisition non-blocking.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new(StringComparer.Ordinal);

    public bool IsRunning(string jobKey)
        => _gates.TryGetValue(jobKey, out var gate) && gate.CurrentCount == 0;

    public Task<TriggerResult> TryTriggerAsync(string jobKey, int userId)
    {
        var descriptor = discovery.Find(jobKey);

        if (descriptor is null || !descriptor.CanTrigger || descriptor.Instance is not IRecurringBackgroundJob job)
        {
            return Task.FromResult(TriggerResult.NotFound);
        }

        if (!RolePermitsExecution(descriptor))
        {
            return Task.FromResult(TriggerResult.RoleNotPermitted);
        }

        var gate = _gates.GetOrAdd(jobKey, _ => new SemaphoreSlim(1, 1));
        if (!gate.Wait(0))
        {
            return Task.FromResult(TriggerResult.AlreadyRunning);
        }

        // Acquired: record the start and run off the request thread. The response is returned
        // immediately; the run continues in the background.
        var record = tracker.RecordStart(jobKey, TriggerSource.Manual, userId);

        _ = Task.Run(async () =>
        {
            try
            {
                await job.RunJobAsync().ConfigureAwait(false);
                tracker.RecordEnd(record, ExecutionOutcome.Success);
            }
            catch (Exception ex)
            {
                tracker.RecordEnd(record, ExecutionOutcome.Failure);
                logger.LogError(ex, "Manual run of job {JobKey} failed.", jobKey);
            }
            finally
            {
                gate.Release();
            }
        });

        return Task.FromResult(TriggerResult.Started);
    }

    private bool RolePermitsExecution(JobDescriptor descriptor)
    {
        var currentRole = serverRoleAccessor.CurrentServerRole;

        // A node that never runs recurring jobs (Subscriber / Unknown) cannot run a manual trigger.
        if (!ServerRoleMapper.NodeExecutesJobs(currentRole))
        {
            return false;
        }

        // If the job declares its server roles, honour them (mirrors Umbraco's own guard).
        if (descriptor.ServerRoles.IsAvailable && descriptor.ServerRoles.Value is { } roles)
        {
            var nodeRole = ServerRoleMapper.ToNodeRole(currentRole);
            return roles.Contains(nodeRole);
        }

        return true;
    }
}
