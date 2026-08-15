# uTPro Background Jobs Monitor for Umbraco

A read-and-trigger management UI for Umbraco recurring background jobs. Surfaces every recurring job registered in your site under a **Settings** dashboard — with timing parameters, execution telemetry, estimated next run, server-role awareness, and an authorized **Run now** action.

Works with **Umbraco 16, 17 and 18**. Optional durable telemetry runs on **SQL Server**, **SQLite** and **PostgreSQL**.

[![NuGet](https://img.shields.io/nuget/v/uTPro.Feature.JobMonitor.svg)](https://www.nuget.org/packages/uTPro.Feature.JobMonitor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/uTPro.Feature.JobMonitor.svg)](https://www.nuget.org/packages/uTPro.Feature.JobMonitor)
[![Umbraco Marketplace](https://img.shields.io/badge/Umbraco-Marketplace-blue)](https://marketplace.umbraco.com/package/utpro.feature.jobmonitor)
[![Umbraco 16+](https://img.shields.io/badge/Umbraco-16%2B-3544B1)](https://umbraco.com)
[![License: Free (proprietary)](https://img.shields.io/badge/License-Free%20(proprietary)-green.svg)](LICENSE.txt)

![Background Jobs Monitor dashboard](https://raw.githubusercontent.com/T4VN/uTPro.Feature.JobMonitor/main/Image/Screenshots/dashboard.png)

---

## Features

- **Auto-discovery** of every recurring background job
- **Timing parameters** — period, delay, server roles
- **Execution telemetry** — last run, duration, outcome
- **Estimated next run** calculation
- **Manual trigger ("Run now")** — overlap guard, role aware
- **Load-balancing aware** — shows current node's server role
- **Configurable storage** — in-memory or durable database table

---

## Quick Start

```bash
dotnet add package uTPro.Feature.JobMonitor
```

Open **Settings → Background Jobs Monitor**. No configuration required.

| Umbraco | .NET | Target |
|---|---|---|
| 16 | .NET 9 | `net9.0` |
| 17 & 18 | .NET 10 | `net10.0` |

---

## Configuration

Under `uTPro:Feature:JobMonitor` in `appsettings.json` (all optional):

| Key | Default | Description |
|---|---|---|
| `Storage` | `InMemory` | `InMemory` or `Durable` |
| `HistoryCapacity` | `50` | Records per job |
| `DiscoveryCacheSeconds` | `30` | Discovery cache duration |

---

## 📖 Full Documentation

**[docs.utpro.dev/uTPro.Feature.JobMonitor](https://docs.utpro.dev/uTPro.Feature.JobMonitor/)**

---

## License

Free to use (including commercially) under a proprietary [End User License Agreement](LICENSE.txt).

---

> 📦 [NuGet](https://www.nuget.org/packages/uTPro.Feature.JobMonitor) · [GitHub](https://github.com/T4VN/uTPro.Feature.JobMonitor) · [Umbraco Marketplace](https://marketplace.umbraco.com/package/utpro.feature.jobmonitor)
