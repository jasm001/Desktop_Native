const implementedCapabilities = [
  "Versioned catalog, request, agent, result, and case APIs",
  "PostgreSQL transactions with append-only audit and outbox",
  "One internal BotCase for every confirmed support request",
  "Idempotent success and failure transitions",
  "Pure 72-hour no-response eligibility policy",
] as const;

const pendingCapabilities = [
  "Typed escalation event",
  "Deterministic ITicketingProvider fake",
  "Synthetic ExternalTicket persistence",
  "Worker processing for fake ticket creation",
] as const;

export default function ControlPlaneStatusPage() {
  return (
    <main className="status-page">
      <header className="status-header">
        <p className="eyebrow">IT Support Native</p>
        <p className="status-badge">Block 8 in progress</p>
        <h1>Internal case foundation</h1>
        <p className="lede">
          The local control plane now creates and tracks one typed internal case
          for every confirmed synthetic request. This is an engineering status
          surface, not the administrative portal.
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
          <p className="section-label">Next increment</p>
          <h2>Fake ticketing</h2>
          <ul>
            {pendingCapabilities.map((capability) => (
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
          This endpoint is read-only. No OpenText connection, Teams channel,
          administrative portal, UEMS integration, or privileged command
          execution is enabled.
        </p>
      </section>
    </main>
  );
}
