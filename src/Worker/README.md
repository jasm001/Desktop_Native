# Worker

Durable Node.js process introduced in Block 7.

The worker claims PostgreSQL outbox rows with `FOR UPDATE SKIP LOCKED`, applies a
bounded retry policy and records one synthetic idempotent effect. For typed case
escalations it invokes `ITicketingProvider`, whose current deterministic fake
implementation persists one synthetic `ExternalTicket` per case. It does not
run inside Next.js, call external services, connect to the DeviceAgent or
execute commands.

Build with `pnpm build`, then run continuously with `pnpm start` or process one
available event with `pnpm start:once`.
