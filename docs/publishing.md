# Publishing

[← Back to README](../README.md)

Release checklist for **NuGet** and the **Umbraco Marketplace**.

## 0. Pre-flight

- [ ] `dotnet build uTPro.Feature.JobMonitor.sln -c Release` is clean (0 warnings, 0 errors).
- [ ] Run the `TestSite` once and confirm the dashboard loads and lists jobs.
- [ ] Bump `<Version>` in `src/uTPro.Feature.JobMonitor/uTPro.Feature.JobMonitor.csproj`.
- [ ] Update `<PackageReleaseNotes>` and the `version` in `wwwroot/umbraco-package.json`.
- [ ] Confirm `README.md`, `icon.png` and `umbraco-marketplace.json` exist at the repo root
      (they are packed into the `.nupkg`).
- [ ] Add screenshots under `Image/Screenshots/` and reference them in `umbraco-marketplace.json`
      (`Screenshots[]`) and the README — the Marketplace listing looks much better with them.
- [ ] Add the schema line back to `umbraco-marketplace.json` (the editor here could not write a
      remote `$schema`):
      `"$schema": "https://marketplace.umbraco.com/umbraco-marketplace-schema.json",`

## 1. Pack

Use the deterministic pack script (wipes bin/obj, evicts the local cache):

```powershell
pwsh ./pack.ps1
```

Output: `Build/uTPro.Feature.JobMonitor.<version>.nupkg`.

Verify the package contents include both target frameworks and the static assets:

```powershell
# lib/net9.0 + lib/net10.0 DLLs, staticwebassets/*, README.md, icon.png, umbraco-marketplace.json
```

## 2. Push to NuGet

```bash
dotnet nuget push Build/uTPro.Feature.JobMonitor.<version>.nupkg \
  --api-key <YOUR_NUGET_API_KEY> \
  --source https://api.nuget.org/v3/index.json
```

Or let CI do it — see `.github/workflows/nuget-release.yml` (pushes on a `v*` tag using the
`NUGET_API_KEY` repository secret).

```bash
git tag v1.0.0
git push origin v1.0.0
```

## 3. List on the Umbraco Marketplace

The Marketplace reads packages from NuGet, so publish to NuGet first.

1. Ensure the NuGet package includes `umbraco-marketplace.json` at the package root (it is, via the
   `<None Include="..\..\umbraco-marketplace.json" Pack="true" PackagePath="/" />` entry).
2. The package must declare Umbraco tags — `umbraco`, `umbraco-marketplace`, `umbraco-package`
   (already in `<PackageTags>`).
3. Go to <https://marketplace.umbraco.com>, sign in, and **submit / claim** the package by its
   NuGet id `uTPro.Feature.JobMonitor`. The Marketplace ingests the metadata, description, tags and
   screenshots from `umbraco-marketplace.json`.
4. Verify the listing (title, description, category, screenshots, documentation & issue links).

## 4. Post-release

- [ ] Create a GitHub Release with notes matching `<PackageReleaseNotes>`.
- [ ] Smoke-test: install the published package into a fresh Umbraco 16 and 18 site.
- [ ] Update the badges/links in `README.md` if the NuGet id or Marketplace slug differ.

## Metadata source of truth

| Artifact | File |
|---|---|
| NuGet id, version, title, description, tags, icon, readme | `src/uTPro.Feature.JobMonitor/uTPro.Feature.JobMonitor.csproj` |
| Marketplace listing (category, screenshots, links) | `umbraco-marketplace.json` |
| Backoffice manifest version | `src/uTPro.Feature.JobMonitor/wwwroot/umbraco-package.json` |
