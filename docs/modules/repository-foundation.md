# Repository Foundation

## Responsibility

Provide a reproducible repository structure and quality baseline without
implementing product behavior.

## Boundaries

- `src/Desktop`: reserved Windows desktop boundary; WinUI starts in Block 1.
- `src/DeviceAgent`: restricted Windows worker host.
- `src/DeviceAgent/Core`: testable IPC, authorization, durable state, and
  simulated jobs without privileged actions.
- `src/Contracts`: versioned cross-process and HTTP contracts.
- `src/BuildingBlocks`: shared primitives with no executable dependencies.
- `src/AdminWeb`: boundary reserved by Block 0 and activated in Block 7 for the
  shared API and persistence; portal UI remains deferred to Block 11.
- `src/Worker`: boundary reserved by Block 0 and activated in Block 7 as a
  separate durable Node.js process.

## Quality Controls

- .NET 10 SDK pinned by `global.json`.
- Centralized NuGet package versions and lock files.
- Nullable reference types, built-in analyzers, and warnings as errors.
- xUnit smoke, contract, and dependency-boundary tests.
- pnpm workspace validation without runtime dependencies.
- Gitleaks directory scan before commit.

## Security

This block adds no shell execution surface, privileged action, external
integration, production endpoint, credential, or corporate data.

## Historical Next Gate

Block 1 may replace the desktop placeholder with the WinUI 3 shell while
preserving the project boundary and quality controls.

This was the next gate when Block 0 closed. Blocks 1 through 7 are now complete;
the current next block is Block 8.
