import { html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { formatDuration, formatTimestamp } from './format.js';

const unavailable = (text = 'Unavailable') => html`<span class="unavailable">${text}</span>`;

function renderTiming(timing) {
    const period = timing?.periodAvailable
        ? html`<div>Period: <strong>${formatDuration(timing.periodSeconds)}</strong></div>`
        : html`<div>Period: ${unavailable()}</div>`;
    const delay = timing?.delayAvailable
        ? html`<div>Delay: <strong>${formatDuration(timing.delaySeconds)}</strong></div>`
        : html`<div>Delay: ${unavailable()}</div>`;
    const roles = timing?.serverRolesAvailable
        ? html`<div>Roles: ${(timing.serverRoles ?? []).join(', ') || unavailable('None')}</div>`
        : html`<div>Roles: ${unavailable()}</div>`;
    return html`${period}${delay}${roles}`;
}

function renderLastRun(job) {
    const run = job.lastRun;
    if (!run) return unavailable('Not yet recorded');
    if (run.isRunning) {
        return html`<div><span class="badge">Running…</span></div>
            <div>${formatTimestamp(run.startUtc)}</div>`;
    }
    const outcomeClass = run.outcome === 'Success' ? 'success' : 'failure';
    return html`
        <div><span class="badge ${outcomeClass}">${run.outcome}</span> <span class="badge">${run.source}</span></div>
        <div>${formatTimestamp(run.startUtc)}</div>
        <div>Took ${formatDuration(run.durationSeconds)}</div>`;
}

function renderEstimate(job) {
    if (!job.estimatedNextRunUtc) return unavailable();
    return html`
        <div>${formatTimestamp(job.estimatedNextRunUtc)}</div>
        <div class="estimate-label">estimate</div>`;
}

function renderModelCell(job) {
    const modelClass = job.model === 'Modern' ? 'modern' : 'legacy';
    return html`
        <div><span class="badge ${modelClass}">${job.model}</span></div>
        ${job.limitedSupport ? html`<div class="estimate-label">Limited monitoring support</div>` : nothing}
        <div class="badges">
            ${(job.capabilities ?? []).map((c) => html`<span class="badge">${c}</span>`)}
        </div>`;
}

function renderRunControl(host, job) {
    if (!job.canTrigger) {
        return html`<uui-button label="Run now" state="" disabled look="secondary"></uui-button>`;
    }
    const busy = host.runningKeys.has(job.key);
    return html`
        <uui-button
            label="Run now"
            look="primary"
            .state=${busy ? 'waiting' : ''}
            ?disabled=${busy}
            @click=${() => host.runNow(job.key)}></uui-button>`;
}

function renderNotices(host) {
    const notices = [];
    if (!host.roleExecutesJobs) {
        notices.push(html`<uui-box><strong>Heads up:</strong> this node's server role
            (<em>${host.serverRole}</em>) does not run scheduled jobs.</uui-box>`);
    }
    if (host.storageMode === 'InMemory') {
        notices.push(html`<uui-box>Execution history is kept <strong>in memory</strong>:
            it resets when the application restarts and reflects only the current node
            (not shared across load-balanced nodes).</uui-box>`);
    }
    notices.push(html`<uui-box><strong>Caution:</strong> "Run now" is advisable only for
        idempotent jobs — a manual run has the same side effects as a scheduled run.</uui-box>`);
    return html`<div class="notices">${notices}</div>`;
}

export function renderDashboard(host) {
    if (host.isLoading) {
        return html`<uui-loader-bar></uui-loader-bar>`;
    }

    return html`
        <div class="header">
            <div>
                <span class="count">${host.count}</span>
                recurring background job${host.count === 1 ? '' : 's'} found
            </div>
            <div>
                Server role: <strong>${host.serverRole}</strong>
                <uui-button label="Refresh" look="secondary" @click=${() => host.reload()}></uui-button>
            </div>
        </div>

        ${renderNotices(host)}

        ${host.count === 0
            ? html`<div class="empty">No recurring background jobs were found.</div>`
            : html`
                <table>
                    <thead>
                        <tr>
                            <th>Job</th>
                            <th>Model &amp; capabilities</th>
                            <th>Timing</th>
                            <th>Last run</th>
                            <th>Est. next run</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${host.jobs.map((job) => html`
                            <tr>
                                <td class="type-name">${job.typeName}</td>
                                <td>${renderModelCell(job)}</td>
                                <td>${renderTiming(job.timing)}</td>
                                <td>${renderLastRun(job)}</td>
                                <td>${renderEstimate(job)}</td>
                                <td>${renderRunControl(host, job)}</td>
                            </tr>`)}
                    </tbody>
                </table>`}`;
}
