# Admin Web

Next.js App Router host for the shared control plane introduced in Block 7 and
the internal case foundation introduced in Block 8.

The current implementation contains versioned synthetic APIs, development
identity, modular application boundaries, Prisma/PostgreSQL infrastructure and
one `BotCase` per confirmed request. Failed results publish a typed escalation
event that the separate worker resolves through a deterministic fake provider
into one synthetic `ExternalTicket`. There is no OpenText connection or
administrative portal. Portal UI, production identity and RBAC remain reserved
for Block 11.

Local runtime configuration is documented in `.env.example`. Values are not
committed in `.env`.
