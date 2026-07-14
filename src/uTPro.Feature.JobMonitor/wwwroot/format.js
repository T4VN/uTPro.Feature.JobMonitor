// Pure formatting helpers (no DOM, no Lit templates).

const UNAVAILABLE = 'Unavailable';

/** Human-readable duration from a number of seconds. Returns "Unavailable" for null/undefined. */
export function formatDuration(totalSeconds) {
    if (totalSeconds === null || totalSeconds === undefined || Number.isNaN(totalSeconds)) return UNAVAILABLE;
    if (totalSeconds === 0) return '0s';

    const negative = totalSeconds < 0;
    let s = Math.floor(Math.abs(totalSeconds));
    const ms = Math.round((Math.abs(totalSeconds) - s) * 1000);

    const days = Math.floor(s / 86400); s -= days * 86400;
    const hours = Math.floor(s / 3600); s -= hours * 3600;
    const minutes = Math.floor(s / 60); s -= minutes * 60;
    const seconds = s;

    const parts = [];
    if (days) parts.push(`${days}d`);
    if (hours) parts.push(`${hours}h`);
    if (minutes) parts.push(`${minutes}m`);
    if (seconds) parts.push(`${seconds}s`);
    if (!parts.length && ms) parts.push(`${ms}ms`);
    if (!parts.length) parts.push('0s');

    return (negative ? '-' : '') + parts.join(' ');
}

/** Format an ISO timestamp in local time with an explicit timezone label. Returns "Unavailable" for empty. */
export function formatTimestamp(iso) {
    if (!iso) return UNAVAILABLE;
    const date = new Date(iso);
    if (Number.isNaN(date.getTime())) return UNAVAILABLE;
    return `${date.toLocaleString()} (${localTimeLabel()})`;
}

/** Label for the browser timezone offset, e.g. "GMT+7". */
export function localTimeLabel() {
    const offsetMinutes = -new Date().getTimezoneOffset();
    const sign = offsetMinutes >= 0 ? '+' : '-';
    const abs = Math.abs(offsetMinutes);
    const hours = Math.floor(abs / 60);
    const minutes = abs % 60;
    return minutes === 0 ? `GMT${sign}${hours}` : `GMT${sign}${hours}:${String(minutes).padStart(2, '0')}`;
}

export { UNAVAILABLE };
