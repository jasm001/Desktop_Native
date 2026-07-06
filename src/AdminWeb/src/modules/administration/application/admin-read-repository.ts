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

export type AdminLabStatus =
  | "available"
  | "offline"
  | "unavailable"
  | "misconfigured"
  | "validate-only"
  | "not_checked";

export type AdminLabSource =
  | "local"
  | "fake"
  | "validate-only"
  | "lab-real-sanitized";

export type AdminLabScope =
  | "development"
  | "local-demo"
  | "validate-only";

export type AdminLabMode =
  | "read-only"
  | "health-check"
  | "validate-only"
  | "not-configured";

export interface AdminLabComponentStatus {
  readonly id: string;
  readonly name: string;
  readonly status: AdminLabStatus;
  readonly source: AdminLabSource;
  readonly scope: AdminLabScope;
  readonly mode: AdminLabMode;
  readonly detail: string;
  readonly lastCheckedAt: Date | null;
}

export interface AdminLabMetric {
  readonly label: string;
  readonly value: number;
  readonly source: AdminLabSource;
  readonly detail: string;
}

export interface AdminLabOutboxSummary {
  readonly id: string;
  readonly eventType: string;
  readonly aggregateType: string;
  readonly status: string;
  readonly attemptCount: number;
  readonly createdAt: Date;
}

export interface AdminLabExternalTicketSummary {
  readonly id: string;
  readonly reference: string;
  readonly provider: string;
  readonly status: string;
  readonly reasonCode: string;
  readonly correlationId: string;
  readonly createdAt: Date;
}

export interface AdminLabTraceStage {
  readonly id: string;
  readonly label: string;
  readonly status: AdminLabStatus;
  readonly source: AdminLabSource;
  readonly detail: string;
  readonly recordedAt: Date | null;
}

export interface AdminLabTraceEvidence {
  readonly code: string;
  readonly summary: string;
  readonly recordedAt: Date;
}

export interface AdminLabTraceOutboxEvent {
  readonly eventType: string;
  readonly aggregateType: string;
  readonly status: string;
  readonly attemptCount: number;
  readonly effectType: string | null;
  readonly createdAt: Date;
}

export interface AdminLabTraceIdempotency {
  readonly requestReplayProtected: boolean;
  readonly resultReplayProtected: boolean;
  readonly duplicateRequestRows: number;
  readonly duplicateResultRows: number;
}

export interface AdminLabTrace {
  readonly requestId: string;
  readonly requestReference: string;
  readonly correlationId: string;
  readonly deviceName: string;
  readonly productName: string;
  readonly requestStatus: string;
  readonly jobId: string | null;
  readonly jobStatus: string | null;
  readonly caseId: string | null;
  readonly caseStatus: string | null;
  readonly ticketReference: string | null;
  readonly createdAt: Date;
  readonly stages: readonly AdminLabTraceStage[];
  readonly evidence: readonly AdminLabTraceEvidence[];
  readonly outboxEvents: readonly AdminLabTraceOutboxEvent[];
  readonly idempotency: AdminLabTraceIdempotency;
}

export interface AdminLabReadModel {
  readonly components: readonly AdminLabComponentStatus[];
  readonly metrics: readonly AdminLabMetric[];
  readonly traces: readonly AdminLabTrace[];
  readonly recentOperations: readonly AdminOperationSummary[];
  readonly recentAuditEvents: readonly AdminAuditSummary[];
  readonly recentOutboxEvents: readonly AdminLabOutboxSummary[];
  readonly recentExternalTickets: readonly AdminLabExternalTicketSummary[];
  readonly boundaries: readonly string[];
}

export interface AdminReadRepository {
  listRecentOperations(limit: number): Promise<readonly AdminOperationSummary[]>;
  listRecentAuditEvents(limit: number): Promise<readonly AdminAuditSummary[]>;
  getLabReadModel(limit: number): Promise<AdminLabReadModel>;
}
