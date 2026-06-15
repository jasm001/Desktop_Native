export interface AdminOperationSummary {
  readonly id: string;
  readonly reference: string;
  readonly deviceName: string;
  readonly productName: string;
  readonly requestStatus: string;
  readonly jobStatus: string | null;
  readonly caseStatus: string | null;
  readonly ticketReference: string | null;
  readonly createdAt: Date;
}

export interface AdminAuditSummary {
  readonly id: string;
  readonly eventType: string;
  readonly entityType: string;
  readonly entityId: string;
  readonly actorSubject: string;
  readonly correlationId: string;
  readonly createdAt: Date;
}

export interface AdminReadRepository {
  listRecentOperations(limit: number): Promise<readonly AdminOperationSummary[]>;
  listRecentAuditEvents(limit: number): Promise<readonly AdminAuditSummary[]>;
}
