# IT Support Native

IT Support Native is a Windows 11 support product built around an unprivileged
WinUI 3 client, a restricted device agent, and a shared control plane. The
repository currently contains only the Block 0 foundation; no privileged action,
production integration, or administrative portal behavior is implemented.

## Current Status

- Block 0: repository foundation completed.
- Next block: native WinUI shell.
- Data policy: synthetic data and deterministic fakes only.
- Remote: `https://github.com/jasm001/Desktop_Native.git`.

The implementation sequence and gates are defined in
[`DEVELOPMENT_PLAN.md`](DEVELOPMENT_PLAN.md). Current execution state is recorded
in [`WORKFLOW.md`](WORKFLOW.md).

## Prerequisites

- Windows 11.
- .NET SDK 10.0.301 or a compatible 10.0 feature band.
- Node.js 24 LTS.
- Corepack.
- Gitleaks 8.x for local secret scanning.
- Visual Studio 2022 with Windows application development tooling for Block 1.

The repository ignores local portable installations under `.dotnet/` and
`.tools/`. They are development conveniences, not source dependencies.

## Repository Layout

```text
src/
  Desktop/          Windows desktop boundary; WinUI starts in Block 1
  DeviceAgent/      Restricted Windows worker with no privileged actions yet
  Contracts/        Versioned shared contracts
  BuildingBlocks/   Dependency-light shared primitives
  AdminWeb/         Reserved until Block 11
  Worker/           Reserved durable Node.js worker boundary
tests/
  Unit/
  Architecture/
  Contract/
  Integration/
  WindowsUi/
  AdminWeb/
deploy/
docs/
```

## Validation

Run the complete local gate from PowerShell:

```powershell
.\scripts\Validate.ps1
```

Equivalent commands:

```powershell
dotnet restore ITSupportNative.slnx --locked-mode
dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
dotnet build ITSupportNative.slnx --configuration Release --no-restore
dotnet test ITSupportNative.slnx --configuration Release --no-build
corepack pnpm@11.5.3 install --frozen-lockfile
corepack pnpm@11.5.3 run check
.\scripts\Test-Secrets.ps1
```

GitHub Actions repeats build, test, format, workspace, and secret-scan gates on
`main` and pull requests.

## Security Baseline

- WinUI, Teams, and AI never execute privileged commands.
- The DeviceAgent does not accept shell text or generated arguments.
- Queries do not create requests, tickets, or installations without explicit
  confirmation.
- Secrets, corporate data, and production endpoints are prohibited in local
  development.
- Actions introduced in later blocks must be typed, authorized, idempotent,
  auditable, and cancellable when safe.

Normative requirements live in [`core/`](core/) and
[`standards/`](standards/).
