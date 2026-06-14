const implementedCapabilities = [
  "Versioned catalog, request, agent, result, and case APIs",
  "PostgreSQL transactions with append-only audit and outbox",
  "One internal BotCase for every confirmed support request",
  "Idempotent success and failure transitions",
  "Typed escalation event emitted with the failed result",
  "Deterministic ITicketingProvider fake",
  "One synthetic ExternalTicket per escalated case",
  "Worker retry processing without duplicate tickets",
  "Pure 72-hour no-response eligibility policy",
] as const;

const protectedBoundaries = [
  "No OpenText network connection or corporate identifiers",
  "No Teams channel implementation before Block 9",
  "No administrative portal behavior before Block 11",
  "No privileged command execution from the control plane",
] as const;

export default function ControlPlaneStatusPage() {
  return (
    <main className="status-page">
      <header className="status-header">
        <p className="eyebrow">IT Support Native</p>
        <p className="status-badge">Block 8 completed</p>
        <h1>Internal cases and fake ticketing</h1>
        <p className="lede">
          The local control plane tracks each confirmed synthetic request,
          escalates failed execution through the durable worker, and exposes one
          deterministic fake ticket. This remains an engineering status surface.
        </p>
      </header>

      <section className="status-grid" aria-label="Block 8 implementation status">
        <article>
          <p className="section-label">Implemented</p>
          <h2>Case lifecycle</h2>
          <ul>
            {implementedCapabilities.map((capability) => (
              <li key={capability}>{capability}</li>
            ))}
          </ul>
        </article>

        <article>
          <p className="section-label">Protected</p>
          <h2>Current boundaries</h2>
          <ul>
            {protectedBoundaries.map((capability) => (
              <li key={capability}>{capability}</li>
            ))}
          </ul>
        </article>
      </section>

      <section className="boundary-note">
        <h2>Current boundary</h2>
        <code className="endpoint">
          GET /api/v1/requests/&#123;requestId&#125;/case
        </code>
        <p>
          This endpoint is read-only and returns the synthetic ticket only after
          worker processing. Block 9 is the next planned unit.
        </p>
      </section>
    </main>
  );
}
