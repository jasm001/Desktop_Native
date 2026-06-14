import type { PoolClient } from "pg";
import type { ClaimedOutboxEvent } from "../../outbox/outbox-store.js";
import type { ExternalTicketDraft } from "../application/ticketing-provider.js";

export async function persistExternalTicket(
  client: PoolClient,
  event: ClaimedOutboxEvent,
  ticket: ExternalTicketDraft,
): Promise<ExternalTicketDraft> {
  const inserted = await client.query<{ id: string }>(
    `
      INSERT INTO external_tickets (
        id,
        case_id,
        provider,
        external_reference,
        category,
        status,
        correlation_id,
        reason_code,
        description,
        created_at
      )
      VALUES (
        $1,
        $2,
        'FAKE',
        $3,
        'SOFTWARE_INSTALLATION',
        'OPEN',
        $4,
        $5,
        $6,
        clock_timestamp()
      )
      ON CONFLICT (case_id) DO NOTHING
      RETURNING id
    `,
    [
      ticket.id,
      ticket.caseId,
      ticket.externalReference,
      ticket.correlationId,
      ticket.reasonCode,
      ticket.description,
    ],
  );

  if (inserted.rowCount === 1) {
    await client.query(
      `
        INSERT INTO audit_events (
          id,
          correlation_id,
          actor_subject,
          event_type,
          entity_type,
          entity_id,
          payload,
          created_at
        )
        VALUES (
          $1,
          $2,
          $3,
          'external-ticket.created',
          'external-ticket',
          $4,
          $5::jsonb,
          clock_timestamp()
        )
      `,
      [
        crypto.randomUUID(),
        ticket.correlationId,
        event.claimedBy,
        ticket.id,
        JSON.stringify({
          ticketId: ticket.id,
          caseId: ticket.caseId,
          provider: ticket.provider,
          externalReference: ticket.externalReference,
          category: ticket.category,
          reasonCode: ticket.reasonCode,
        }),
      ],
    );
    return ticket;
  }

  const existing = await client.query<{
    id: string;
    external_reference: string;
    correlation_id: string;
    reason_code: ExternalTicketDraft["reasonCode"];
    description: string;
  }>(
    `
      SELECT
        id,
        external_reference,
        correlation_id,
        reason_code,
        description
      FROM external_tickets
      WHERE case_id = $1
    `,
    [ticket.caseId],
  );
  const row = existing.rows[0];
  if (
    row === undefined ||
    row.id !== ticket.id ||
    row.external_reference !== ticket.externalReference ||
    row.correlation_id !== ticket.correlationId ||
    row.reason_code !== ticket.reasonCode ||
    row.description !== ticket.description
  ) {
    throw new Error("external_ticket_conflict");
  }

  return ticket;
}
