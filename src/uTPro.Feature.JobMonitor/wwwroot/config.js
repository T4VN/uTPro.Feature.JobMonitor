// Static configuration for the Job Monitor dashboard.

export const API_BASE = '/umbraco/management/api/v1/utpro/job-monitor';

export const ENDPOINTS = {
    jobs: 'jobs',
    history: (key) => `jobs/${encodeURIComponent(key)}/history`,
    run: (key) => `jobs/${encodeURIComponent(key)}/run`
};
