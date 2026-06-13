export interface AuditEvent {
  readonly correlationId: string;
  readonly actorSubject: string;
  readonly eventType: string;
  readonly entityType: string;
  readonly entityId: string;
  readonly payload: Readonly<Record<string, unknown>>;
}
