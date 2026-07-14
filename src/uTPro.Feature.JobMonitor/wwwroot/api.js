// Thin wrapper around the Management API, handling backoffice auth headers.

import { API_BASE } from './config.js';

async function authHeaders(authContext) {
    const config = authContext?.getOpenApiConfiguration();
    const headers = { 'Content-Type': 'application/json' };
    if (config?.token) {
        const token = await config.token();
        if (token) headers['Authorization'] = `Bearer ${token}`;
    }
    return { headers, credentials: config?.credentials || 'same-origin' };
}

/** GET an endpoint and return the parsed JSON body. */
export async function getJson(authContext, endpoint) {
    const { headers, credentials } = await authHeaders(authContext);
    const response = await fetch(`${API_BASE}/${endpoint}`, { method: 'GET', headers, credentials });
    if (!response.ok) throw new Error(`API error: ${response.status}`);
    return response.json();
}

/**
 * POST to an endpoint. Returns { status, body } so the caller can react to
 * 202 (started) vs 409 (already-running / role-not-permitted) etc.
 */
export async function postAction(authContext, endpoint) {
    const { headers, credentials } = await authHeaders(authContext);
    const response = await fetch(`${API_BASE}/${endpoint}`, { method: 'POST', headers, credentials, body: '{}' });
    let body = null;
    try { body = await response.json(); } catch { /* no body */ }
    return { ok: response.ok, status: response.status, body };
}
