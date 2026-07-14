import { css } from '@umbraco-cms/backoffice/external/lit';

export const dashboardStyles = css`
    :host {
        display: block;
        padding: var(--uui-size-layout-1, 24px);
    }

    .header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: var(--uui-size-space-3, 12px);
        margin-bottom: var(--uui-size-space-4, 16px);
    }

    .count {
        font-weight: 700;
    }

    .notices {
        display: flex;
        flex-direction: column;
        gap: var(--uui-size-space-2, 8px);
        margin-bottom: var(--uui-size-space-4, 16px);
    }

    table {
        width: 100%;
        border-collapse: collapse;
        background: var(--uui-color-surface, #fff);
        font-size: 0.9rem;
    }

    th, td {
        text-align: left;
        padding: var(--uui-size-space-3, 12px);
        border-bottom: 1px solid var(--uui-color-border, #e9e9eb);
        vertical-align: top;
    }

    th {
        font-weight: 700;
        white-space: nowrap;
    }

    .type-name {
        font-family: var(--uui-font-monospace, monospace);
        word-break: break-all;
    }

    .badges {
        display: flex;
        flex-wrap: wrap;
        gap: 4px;
        margin-top: 4px;
    }

    .badge {
        display: inline-block;
        padding: 1px 8px;
        border-radius: 10px;
        font-size: 0.72rem;
        line-height: 1.5;
        background: var(--uui-color-surface-alt, #f3f3f5);
        border: 1px solid var(--uui-color-border, #e9e9eb);
    }

    .badge.modern { background: #e6f4ea; }
    .badge.legacy { background: #fdf2e0; }
    .badge.success { background: #e6f4ea; }
    .badge.failure { background: #fce8e6; }

    .unavailable {
        color: var(--uui-color-text-alt, #868692);
        font-style: italic;
    }

    .estimate-label {
        color: var(--uui-color-text-alt, #868692);
        font-size: 0.75rem;
    }

    .empty {
        padding: var(--uui-size-space-6, 24px);
        text-align: center;
        color: var(--uui-color-text-alt, #868692);
    }
`;
