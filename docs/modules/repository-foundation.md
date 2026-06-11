# Repository Foundation

## Responsibility

Provide a reproducible repository structure and quality baseline without
implementing product behavior.

## Boundaries

- `src/Desktop`: reserved Windows desktop boundary; WinUI starts in Block 1.
- `src/DeviceAgent`: restricted Windows worker process with no privileged
  actions in Block 0.
- `src/Contracts`: versioned cross-process and HTTP contracts.
- `src/BuildingBlocks`: shared primitives with no executable dependencies.
- `src/AdminWeb`: reserved portal boundary; implementation starts in Block 11.
- `src/Worker`: reserved durable Node.js worker boundary.

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

## Next Gate

Block 1 may replace the desktop placeholder with the WinUI 3 shell while
preserving the project boundary and quality controls.
