# Admin Web

Next.js App Router host for the shared control plane introduced in Block 7 and
the internal case foundation introduced in Block 8.

The current implementation contains versioned synthetic APIs, development
identity, modular application boundaries, Prisma/PostgreSQL infrastructure and
one `BotCase` per confirmed request. Failed results publish a typed escalation
event that the separate worker resolves through a deterministic fake provider
into one synthetic `ExternalTicket`. There is no OpenText connection or
production administrative portal.

Block 11 is `in_progress`. The root page remains an engineering status surface.
The protected `/admin`, `/admin/catalog`, `/admin/operations`, `/admin/audit`,
`/admin/access`, `/admin/approvals`, `/admin/support`, `/admin/reporting`, and
`/admin/lab` routes use a separate synthetic portal identity, fail-closed
server-side read capabilities, bounded Prisma projections or in-memory
synthetic states, and an accessible read-only shell. `/admin/lab` reads local
PostgreSQL counts and summaries as `lab-real-sanitized` data. They do not add
OIDC/Entra, production RBAC, Fluent UI, mutations, or corporate integrations.

Local runtime configuration is documented in `.env.example`. Values are not
committed in `.env`.
