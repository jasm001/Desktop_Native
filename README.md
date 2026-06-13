# IT Support Native

IT Support Native is a Windows 11 support product built around an unprivileged
WinUI 3 client, a restricted device agent, and a shared control plane. The
repository currently contains the Block 0 foundation, the Block 1 native shell,
the Block 2 synthetic catalog domain, and the Block 3 deterministic
conversation flow. Block 4 adds a simulated device agent with typed local IPC
and durable synthetic jobs. Block 5 adds bounded read-only device diagnostics
with partial typed results. Block 6 adds one closed 7-Zip MSI adapter validated
only in a disposable Windows 11 VM. Block 7 completes the shared control plane,
PostgreSQL persistence, durable worker, and local simulated-agent flow. No
production integration or administrative portal behavior is implemented.

## Current Status

- Block 0: repository foundation completed.
- Block 1: native WinUI shell completed.
- Block 2: synthetic catalog domain and deterministic rules completed.
- Block 3: deterministic conversation states and idempotent synthetic requests
  completed.
- Block 4: simulated device agent, typed IPC, SQLite recovery, and deny-by-default
  authorization completed.
- Block 5: bounded read-only diagnostics and typed action prerequisites
  completed.
- Block 6: closed 7-Zip 26.01 x64 adapter and disposable VM matrix completed.
- Block 7: shared API, PostgreSQL persistence, transactional outbox, durable
  worker, and local WinUI-to-simulated-agent flow completed.
- Next block: Block 8, internal cases, escalation, 72-hour closure policy, and
  a deterministic fake `ITicketingProvider`; real OpenText remains disabled.
- Local demonstration profile: Windows 11 VM, public/synthetic data, local or
  fake providers, a development artifact mirror, and optional Hermes/RAG. This
  profile is not a corporate pilot.
- Data policy: synthetic data and deterministic fakes only.
- Remote: `https://github.com/jasm001/Desktop_Native.git`.

The implementation sequence and gates are defined in
[`DEVELOPMENT_PLAN.md`](DEVELOPMENT_PLAN.md). Current execution state is recorded
in [`WORKFLOW.md`](WORKFLOW.md). The bounded local demonstration is defined in
[`docs/modules/local-mvp-lab.md`](docs/modules/local-mvp-lab.md).

## Documentation Precedence

When documents disagree, do not choose silently. Use this order and register a
stopper in `WORKFLOW.md` when the conflict affects scope, security, architecture,
contracts, or delivery:

1. `core/SECURITY.md` and closed decisions in `core/DECISIONS.md`;
2. `core/SCOPE.md`, `core/STACK.md`, and `core/ARCHITECTURE.md`;
3. `standards/`;
4. the owning document under `modules/`;
5. `DEVELOPMENT_PLAN.md`;
6. `CURRENT_CONTEXT.md` and `WORKFLOW.md` for current execution state;
7. `reference/` and historical material.

`MASTER_PROMPT.md` is a handoff aid, not a normative source. A user request that
conflicts with a higher-precedence document requires an explicit decision or
stopper before implementation.

## Prerequisites

- Windows 11.
- .NET SDK 10.0.301 or a compatible 10.0 feature band.
- Node.js 24 LTS.
- Corepack.
- PostgreSQL 18 for the Block 7 integration gate. `DATABASE_URL` must target a
  non-production database and the role must have `CREATEDB`.
- Gitleaks 8.x for local secret scanning.
- Visual Studio Community 2026 version 18.0 or later for interactive WinUI
  development.

Visual Studio 2022 version 17.x cannot load projects that target `net10.0`,
even when the .NET 10 SDK is installed globally. Visual Studio 2026 can be
installed side-by-side with Visual Studio 2022.

The repository ignores local portable installations under `.dotnet/` and
`.tools/`. They are development conveniences, not source dependencies.

## Repository Layout

```text
src/
  Catalog/          Pure synthetic software catalog and decisions
  Conversation/     Deterministic conversation state machine
  Desktop/          WinUI 3 shell with five local synthetic views
  DeviceAgent/      Restricted Windows worker and testable simulated agent core
  Contracts/        Versioned shared IPC and HTTP contracts
  BuildingBlocks/   Dependency-light shared primitives
  AdminWeb/         Next.js control plane API and PostgreSQL infrastructure
  Worker/           Separate durable Node.js outbox worker
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
dotnet restore ITSupportNative.slnx --locked-mode -m:1 --disable-build-servers
dotnet format ITSupportNative.slnx --verify-no-changes --no-restore
dotnet restore ITSupportNative.slnx --locked-mode -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
dotnet build ITSupportNative.slnx --configuration Release --no-restore -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
dotnet test ITSupportNative.slnx --configuration Release --no-build -m:1 `
  --artifacts-path .artifacts/validate --disable-build-servers
corepack pnpm@11.5.3 install --frozen-lockfile
corepack pnpm@11.5.3 run check
corepack pnpm@11.5.3 run test:integration
.\scripts\Test-Secrets.ps1
```

The integration command creates a random PostgreSQL database, applies
`prisma migrate deploy`, seeds synthetic development data, runs AdminWeb and
worker integration tests, and removes the database. GitHub Actions repeats the
build, test, format, PostgreSQL integration, workspace, and secret-scan gates on
`main` and pull requests.

Run the native shell after a Release build:

```powershell
.\src\Desktop\bin\Release\net10.0-windows10.0.26100.0\win-x64\ITSupportNative.Desktop.exe
```

The development executable is self-contained for .NET and Windows App SDK. It
runs as the current user and does not perform privileged actions.

For a direct Debug build and run from PowerShell:

```powershell
dotnet restore ITSupportNative.slnx --locked-mode -m:1 --disable-build-servers
dotnet run --project .\src\Desktop\ITSupportNative.Desktop.csproj `
  --configuration Debug --no-restore
```

If Visual Studio reports that `Microsoft.NET.Sdk` cannot be found, verify the
IDE version first. For this repository, `dotnet --info` must resolve SDK
`10.0.301`, and the IDE must be Visual Studio 2026 version 18.0 or later.

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
