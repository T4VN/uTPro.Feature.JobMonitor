# uTPro Background Jobs Monitor for Umbraco

A read-and-trigger management UI for Umbraco recurring background jobs. Surfaces every recurring
job registered in your site under a **Settings** dashboard — with timing parameters, execution
telemetry the CMS does not persist natively, an estimated next run, server-role awareness, and an
authorized on-demand **Run now** action.

Works with **Umbraco 16, 17 and 18** (multi-targeted `net9.0` / `net10.0`).

Optional durable telemetry runs on **SQL Server**, **SQLite** and **PostgreSQL**.

[![NuGet](https://img.shields.io/nuget/v/uTPro.Feature.JobMonitor.svg)](https://www.nuget.org/packages/uTPro.Feature.JobMonitor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/uTPro.Feature.JobMonitor.svg)](https://www.nuget.org/packages/uTPro.Feature.JobMonitor)
[![Umbraco Marketplace](https://img.shields.io/badge/Umbraco-Marketplace-blue)](https://marketplace.umbraco.com/package/utpro.feature.jobmonitor)
[![Umbraco 16+](https://img.shields.io/badge/Umbraco-16%2B-3544B1)](https://umbraco.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

![Background Jobs Monitor dashboard](https://raw.githubusercontent.com/T4VN/uTPro.Feature.JobMonitor/main/Image/Screenshots/dashboard.png)

---

## Features

- **Auto-discovery** of every recurring background job — no per-job wiring.
  - Modern jobs implementing `IRecurringBackgroundJob` (full support).
  - Legacy jobs deriving from `RecurringHostedServiceBase` (listed with limited support).
  - Plain `IHostedService` registrations are excluded.
- **Timing parameters** per job: period, delay and server roles (human-readable durations).
- **Execution telemetry** the CMS does not persist natively: last run start, duration and outcome.
- **Estimated next run** = most recent run start + period (clearly labelled as an estimate).
- **Manual trigger ("Run now")** for modern jobs — runs off the request thread, guards against
  overlap, honours the node's server role, and records the initiating backoffice user.
- **Load-balancing aware** — shows the current node's server role and warns when scheduled jobs do
  not run on the node, or when history is in-memory / per-node only.
- **Configurable storage** — in-memory ring buffer (default) or a durable database table that
  survives restarts and is shared across nodes.
- **Secure by default** — every endpoint requires access to the Settings section.

---

## Quick Start

```bash
dotnet add package uTPro.Feature.JobMonitor
```

Start Umbraco and open **Settings → Background Jobs Monitor**. Your recurring jobs appear
automatically — no configuration or per-job registration required.

| Umbraco | .NET | Target |
|---|---|---|
| 16 | .NET 9 | `net9.0` |
| 17 & 18 | .NET 10 | `net10.0` |

---

## Configuration

All settings are optional, under `uTPro:Feature:JobMonitor` in `appsettings.json`:

```json
{
  "uTPro": {
    "Feature": {
      "JobMonitor": {
        "Storage": "InMemory",
        "HistoryCapacity": 50,
        "DiscoveryCacheSeconds": 30
      }
    }
  }
}
```

| Key | Default | Description |
|---|---|---|
| `Storage` | `InMemory` | `InMemory` = per-process ring buffer (resets on restart, per-node). `Durable` = persisted to the Umbraco database (survives restarts, shared across load-balanced nodes). |
| `HistoryCapacity` | `50` | Max execution records retained per job. |
| `DiscoveryCacheSeconds` | `30` | How long a discovery result is cached before re-enumeration. `0` disables caching. |

See [Configuration](docs/configuration.md) and [Telemetry & Storage](docs/telemetry-and-storage.md).

---

## Database support (durable storage)

When `Storage` is `Durable`, a state-keyed migration creates the `uTProJobExecution` table on
startup (once per database). Data access uses NPoco strongly-typed queries with provider-quoted
identifiers, so it runs on **SQL Server, SQLite and PostgreSQL** — the same cross-database approach
used by `uTPro.Feature.AuditLog` and `uTPro.Feature.SimpleFormBuilder`. When durable storage is
disabled (the default) the package touches no schema.

---

## Documentation

| Guide | What's inside |
|---|---|
| [Getting Started](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/getting-started.md) | Install, compatibility, backoffice location, what each column means |
| [Configuration](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/configuration.md) | All `appsettings` keys and their effects |
| [Telemetry & Storage](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/telemetry-and-storage.md) | In-memory vs durable, the `uTProJobExecution` table, PostgreSQL, load-balancing |
| [Manual Trigger (Run now)](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/manual-trigger.md) | How Run now works, overlap/role guards, idempotency caution |
| [Security & Permissions](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/security.md) | Authorization model, what is read-only, accountability |
| [Reference](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/reference.md) | Project structure, API endpoints, discovery internals, database schema |
| [Publishing](https://github.com/T4VN/uTPro.Feature.JobMonitor/blob/main/docs/publishing.md) | Release checklist for NuGet and the Umbraco Marketplace |

---

## Building from source

```bash
dotnet build uTPro.Feature.JobMonitor.sln
```

`src/uTPro.Feature.JobMonitor.TestSite` is a runnable Umbraco 18 site (SQLite, unattended install)
that registers example modern / legacy / plain services to exercise the dashboard.

Produce a NuGet package deterministically:

```powershell
pwsh ./pack.ps1
```

---

## License & Author

MIT © [T4VN](https://github.com/T4VN). Issues and contributions welcome on the
[GitHub repository](https://github.com/T4VN/uTPro.Feature.JobMonitor).
