using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Web.Common.Authorization;
using uTPro.Feature.JobMonitor.Models;
using uTPro.Feature.JobMonitor.Services;

namespace uTPro.Feature.JobMonitor.Controllers;

/// <summary>
/// Versioned, authenticated Management API for the Job Monitor dashboard.
/// Every action requires access to the Settings section.
/// </summary>
[VersionedApiBackOfficeRoute("utpro/job-monitor")]
[MapToApi(ConfigureJobMonitorSwaggerGenOptions.ApiName)]
[ApiExplorerSettings(GroupName = "Background Jobs Monitor")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class JobMonitorApiController(
    IJobDiscoveryService discovery,
    IExecutionTracker tracker,
    INextRunEstimator estimator,
    IManualTriggerService trigger,
    IServerRoleAccessor serverRoleAccessor,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : ManagementApiControllerBase
{
    [HttpGet("jobs")]
    [ProducesResponseType(typeof(JobListResponse), StatusCodes.Status200OK)]
    public IActionResult GetJobs()
    {
        var currentRole = serverRoleAccessor.CurrentServerRole;
        var jobs = discovery.DiscoverJobs()
            .Select(ToViewModel)
            .ToList();

        return Ok(new JobListResponse
        {
            Count = jobs.Count,
            ServerRole = ServerRoleMapper.ToNodeRole(currentRole).ToString(),
            RoleExecutesJobs = ServerRoleMapper.NodeExecutesJobs(currentRole),
            StorageMode = tracker.StorageMode.ToString(),
            Jobs = jobs
        });
    }

    [HttpGet("jobs/{key}/history")]
    [ProducesResponseType(typeof(IEnumerable<ExecutionRecordViewModel>), StatusCodes.Status200OK)]
    public IActionResult GetHistory(string key)
    {
        if (discovery.Find(key) is null)
        {
            return NotFound();
        }

        var history = tracker.GetHistory(key)
            .Select(ExecutionRecordViewModel.From)
            .ToList();

        return Ok(history);
    }

    [HttpPost("jobs/{key}/run")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Run(string key)
    {
        var userId = backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id ?? -1;
        var result = await trigger.TryTriggerAsync(key, userId);

        return result switch
        {
            TriggerResult.Started => Accepted(new { status = "started" }),
            TriggerResult.AlreadyRunning => Conflict(new { status = "already-running" }),
            TriggerResult.RoleNotPermitted => Conflict(new { status = "role-not-permitted" }),
            _ => NotFound(new { status = "not-found" })
        };
    }

    private JobViewModel ToViewModel(JobDescriptor descriptor)
    {
        var latest = tracker.GetLatest(descriptor.JobKey);
        var estimate = estimator.Estimate(descriptor.JobKey, descriptor.Period);

        return new JobViewModel
        {
            Key = descriptor.JobKey,
            TypeName = descriptor.TypeName,
            Model = descriptor.Model.ToString(),
            LimitedSupport = descriptor.Model == JobModel.Legacy,
            CanTrigger = descriptor.CanTrigger,
            Timing = ToTimingViewModel(descriptor),
            LastRun = latest is null ? null : ToLastRunViewModel(latest),
            EstimatedNextRunUtc = estimate.IsAvailable ? estimate.Value.ToString("o") : null,
            Capabilities = descriptor.Capabilities
                .ToString()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        };
    }

    private static TimingViewModel ToTimingViewModel(JobDescriptor descriptor) => new()
    {
        PeriodAvailable = descriptor.Period.IsAvailable,
        PeriodSeconds = descriptor.Period.IsAvailable ? descriptor.Period.Value.TotalSeconds : null,
        DelayAvailable = descriptor.Delay.IsAvailable,
        DelaySeconds = descriptor.Delay.IsAvailable ? descriptor.Delay.Value.TotalSeconds : null,
        ServerRolesAvailable = descriptor.ServerRoles.IsAvailable,
        ServerRoles = descriptor.ServerRoles.IsAvailable
            ? descriptor.ServerRoles.Value?.Select(r => r.ToString()).ToArray()
            : null
    };

    private static LastRunViewModel ToLastRunViewModel(ExecutionRecord record) => new()
    {
        StartUtc = record.StartUtc.ToString("o"),
        DurationSeconds = record.Duration?.TotalSeconds,
        Outcome = record.Outcome?.ToString(),
        Source = record.Source.ToString(),
        IsRunning = !record.IsComplete
    };
}
