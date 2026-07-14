import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html } from '@umbraco-cms/backoffice/external/lit';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { ENDPOINTS } from './config.js';
import { getJson, postAction } from './api.js';
import { dashboardStyles } from './styles.js';
import { renderDashboard } from './render.js';

const RUN_STATUS_MESSAGES = {
    'started': { color: 'positive', message: 'Run started.' },
    'already-running': { color: 'warning', message: 'The job is already running.' },
    'role-not-permitted': { color: 'warning', message: "This node's server role does not permit running jobs." },
    'not-found': { color: 'danger', message: 'Job not found or cannot be triggered.' }
};

/**
 * Job Monitor dashboard. Lists recurring background jobs with their timing, last run,
 * estimated next run and a "Run now" action for modern jobs.
 */
export class UtproJobMonitorDashboard extends UmbLitElement {

    static properties = {
        isLoading: { state: true },
        jobs: { state: true },
        count: { state: true },
        serverRole: { state: true },
        roleExecutesJobs: { state: true },
        storageMode: { state: true },
        runningKeys: { state: true }
    };

    static styles = dashboardStyles;

    #authContext;
    #notificationContext;

    constructor() {
        super();
        this.isLoading = true;
        this.jobs = [];
        this.count = 0;
        this.serverRole = 'Unknown';
        this.roleExecutesJobs = false;
        this.storageMode = 'InMemory';
        this.runningKeys = new Set();

        this.consumeContext(UMB_AUTH_CONTEXT, (ctx) => { this.#authContext = ctx; });
        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (ctx) => { this.#notificationContext = ctx; });
    }

    async connectedCallback() {
        super.connectedCallback();
        await this.reload();
    }

    async reload() {
        this.isLoading = true;
        try {
            const data = await getJson(this.#authContext, ENDPOINTS.jobs);
            this.jobs = data.jobs ?? [];
            this.count = data.count ?? this.jobs.length;
            this.serverRole = data.serverRole ?? 'Unknown';
            this.roleExecutesJobs = data.roleExecutesJobs ?? false;
            this.storageMode = data.storageMode ?? 'InMemory';
        } catch (error) {
            console.error('Failed to load jobs:', error);
            this.#notify('danger', 'Failed to load jobs.');
            this.jobs = [];
            this.count = 0;
        }
        this.isLoading = false;
    }

    async runNow(key) {
        this.runningKeys = new Set(this.runningKeys).add(key);
        try {
            const result = await postAction(this.#authContext, ENDPOINTS.run(key));
            const status = result.body?.status ?? (result.ok ? 'started' : 'not-found');
            const info = RUN_STATUS_MESSAGES[status] ?? RUN_STATUS_MESSAGES['not-found'];
            this.#notify(info.color, info.message);
            // Give the background run a moment to record its start, then refresh.
            setTimeout(() => this.reload(), 800);
        } catch (error) {
            console.error('Run failed:', error);
            this.#notify('danger', 'Run request failed.');
        } finally {
            const next = new Set(this.runningKeys);
            next.delete(key);
            this.runningKeys = next;
        }
    }

    #notify(color, message) {
        this.#notificationContext?.peek(color, { data: { message } });
    }

    render() {
        return html`${renderDashboard(this)}`;
    }
}

customElements.define('utpro-job-monitor-dashboard', UtproJobMonitorDashboard);
export default UtproJobMonitorDashboard;
