import {
  executionJobResultRecordedEventType,
  supportRequestConfirmedEventType,
  type ClaimedAgentJob,
  type ExecutionJobResultRecordedEventV1,
  type ReportAgentJobResultRequest,
} from "@it-support-native/control-plane-contracts";
import { createHash, randomUUID } from "node:crypto";
import {
  Prisma,
  type PrismaClient,
} from "../../../generated/prisma/client";
import { ApplicationError } from "../../../platform/errors/application-error";
import { mapSupportRequest } from "../../requests/infrastructure/support-request-mapper";
import type {
  AgentJobRepository,
  ReportedAgentJobResult,
} from "../application/agent-job-repository";

const maximumAttempts = 3;
const leaseSeconds = 30;

const evidenceSummaries = {
  "job.accepted":
    "The job was accepted by the installed development policy.",
  "job.simulation.started":
    "The agent started a simulated execution without invoking an installer.",
  "job.simulation.verified":
    "The simulated verification completed; no device change was performed.",
  "job.simulation.failed":
    "The simulated job failed without exposing internal details.",
} as const;

interface ClaimedRow {
  readonly job_id: string;
  readonly request_id: string;
  readonly idempotency_key: string;
  readonly action_id: string;
  readonly target_id: string;
  readonly target_version: string;
  readonly lease_expires_at: Date;
}

interface ExpiredRow {
  readonly job_id: string;
  readonly request_id: string;
  readonly correlation_id: string;
  readonly device_id: string;
}

export class PrismaAgentJobRepository implements AgentJobRepository {
  public constructor(private readonly prisma: PrismaClient) {}

  public async claimNext(
    deviceId: string,
    agentSubject: string,
    correlationId: string,
  ): Promise<ClaimedAgentJob | null> {
    return this.prisma.$transaction(
      async (transaction) => {
        await failExhaustedClaims(transaction, agentSubject);

        const claimToken = randomUUID();
        const claimTokenHash = hashValue(claimToken);
        const rows = await transaction.$queryRaw<ClaimedRow[]>`
          WITH candidate AS (
            SELECT
              job.id,
              request.id AS request_id,
              request.idempotency_key,
              job.action_id,
              job.target_id,
              job.target_version
            FROM execution_jobs AS job
            JOIN support_requests AS request ON request.id = job.request_id
            WHERE request.device_id = ${deviceId}
              AND job.attempt_count < ${maximumAttempts}
              AND (
                job.status = 'QUEUED'
                OR (
                  job.status = 'RUNNING'
                  AND job.lease_expires_at < clock_timestamp()
                )
              )
              AND EXISTS (
                SELECT 1
                FROM outbox_events AS event
                JOIN synthetic_outbox_effects AS effect
                  ON effect.outbox_event_id = event.id
                WHERE event.aggregate_type = 'support-request'
                  AND event.aggregate_id = request.id::text
                  AND event.event_type = ${supportRequestConfirmedEventType}
                  AND event.status = 'COMPLETED'
                  AND effect.effect_type = 'support-request.dispatch-ready.v1'
              )
            ORDER BY job.created_at, job.id
            FOR UPDATE OF job SKIP LOCKED
            LIMIT 1
          )
          UPDATE execution_jobs AS job
          SET
            status = 'RUNNING',
            attempt_count = job.attempt_count + 1,
            claimed_at = clock_timestamp(),
            claimed_by = ${agentSubject},
            claim_token_hash = ${claimTokenHash},
            lease_expires_at =
              clock_timestamp() + (${leaseSeconds} * interval '1 second')
          FROM candidate
          WHERE job.id = candidate.id
          RETURNING
            job.id AS job_id,
            candidate.request_id,
            candidate.idempotency_key,
            candidate.action_id,
            candidate.target_id,
            candidate.target_version,
            job.lease_expires_at
        `;
        const row = rows[0];
        if (row === undefined) {
          return null;
        }

        await transaction.auditEvent.create({
          data: {
            id: randomUUID(),
            correlationId,
            actorSubject: agentSubject,
            eventType: "execution-job.claimed",
            entityType: "execution-job",
            entityId: row.job_id,
            payload: {
              jobId: row.job_id,
              requestId: row.request_id,
              deviceId,
            },
          },
        });

        return {
          jobId: row.job_id,
          requestId: row.request_id,
          idempotencyKey: row.idempotency_key,
          actionId: row.action_id,
          targetId: row.target_id,
          targetVersion: row.target_version,
          claimToken,
          leaseExpiresAt: row.lease_expires_at.toISOString(),
        };
      },
      transactionOptions,
    );
  }

  public async reportResult(
    jobId: string,
    idempotencyKey: string,
    correlationId: string,
    agentSubject: string,
    result: ReportAgentJobResultRequest,
  ): Promise<ReportedAgentJobResult> {
    const payloadHash = hashResult(result);

    return this.prisma.$transaction(
      async (transaction) => {
        await transaction.$queryRaw`
          SELECT
            pg_advisory_xact_lock(hashtextextended(${jobId}, 0)) IS NULL
              AS acquired
        `;

        const existing = await transaction.executionJob.findUnique({
          where: { id: jobId },
          include: {
            request: true,
            evidence: true,
          },
        });

        if (existing === null) {
          throw invalidClaim();
        }

        if (existing.resultIdempotencyKey !== null) {
          if (
            existing.resultIdempotencyKey !== idempotencyKey ||
            existing.resultPayloadHash !== payloadHash
          ) {
            throw new ApplicationError(
              "idempotency_conflict",
              409,
              "The idempotency key was already used for another result.",
            );
          }

          const request = await loadRequest(transaction, existing.requestId);
          return { request: mapSupportRequest(request), replayed: true };
        }

        const claimValidity = await transaction.$queryRaw<
          Array<{ valid: boolean }>
        >`
          SELECT EXISTS (
            SELECT 1
            FROM execution_jobs
            WHERE id = ${jobId}::uuid
              AND status = 'RUNNING'
              AND claimed_by = ${agentSubject}
              AND claim_token_hash = ${hashValue(result.claimToken)}
              AND lease_expires_at >= clock_timestamp()
          ) AS valid
        `;
        if (claimValidity[0]?.valid !== true) {
          throw invalidClaim();
        }

        const completedStatus =
          result.result === "succeeded" ? "COMPLETED" : "FAILED";
        const requestStatus =
          result.result === "succeeded" ? "COMPLETED" : "FAILED";

        await transaction.$executeRaw`
          UPDATE execution_jobs
          SET
            status = ${completedStatus}::"JobStatus",
            claimed_at = NULL,
            claimed_by = NULL,
            claim_token_hash = NULL,
            lease_expires_at = NULL,
            result_idempotency_key = ${idempotencyKey},
            result_payload_hash = ${payloadHash},
            completed_at = clock_timestamp()
          WHERE id = ${jobId}::uuid
        `;

        for (const [index, evidence] of result.evidence.entries()) {
          await transaction.$executeRaw`
            INSERT INTO execution_evidence (
              id,
              job_id,
              sequence,
              code,
              summary,
              recorded_at,
              created_at
            )
            VALUES (
              ${randomUUID()}::uuid,
              ${jobId}::uuid,
              ${index + 1},
              ${evidence.code},
              ${evidenceSummaries[evidence.code]},
              ${evidence.recordedAt}::timestamptz,
              clock_timestamp()
            )
          `;
        }

        await transaction.supportRequest.update({
          where: { id: existing.requestId },
          data: { status: requestStatus },
        });

        const botCase = await transitionBotCase(
          transaction,
          existing.requestId,
          result.result,
        );

        await transaction.auditEvent.create({
          data: {
            id: randomUUID(),
            correlationId,
            actorSubject: agentSubject,
            eventType: "execution-job.result-recorded",
            entityType: "execution-job",
            entityId: jobId,
            payload: {
              jobId,
              requestId: existing.requestId,
              deviceId: existing.request.deviceId,
              result: result.result,
              evidenceCodes: result.evidence.map((item) => item.code),
            },
          },
        });

        await transaction.auditEvent.create({
          data: {
            id: randomUUID(),
            correlationId,
            actorSubject: agentSubject,
            eventType: "bot-case.result-recorded",
            entityType: "bot-case",
            entityId: botCase.id,
            payload: {
              caseId: botCase.id,
              requestId: existing.requestId,
              result: result.result,
              status:
                result.result === "succeeded"
                  ? "attended_waiting_user"
                  : "escalated",
            },
          },
        });

        await insertResultOutbox(transaction, {
          version: 1,
          requestId: existing.requestId,
          jobId,
          correlationId,
          deviceId: existing.request.deviceId,
          result: result.result,
        });

        const request = await loadRequest(transaction, existing.requestId);
        return { request: mapSupportRequest(request), replayed: false };
      },
      transactionOptions,
    );
  }
}

const transactionOptions = {
  isolationLevel: Prisma.TransactionIsolationLevel.Serializable,
  maxWait: 5_000,
  timeout: 10_000,
} as const;

async function loadRequest(
  transaction: Prisma.TransactionClient,
  requestId: string,
) {
  return transaction.supportRequest.findUniqueOrThrow({
    where: { id: requestId },
    include: { job: { include: { evidence: true } } },
  });
}

async function failExhaustedClaims(
  transaction: Prisma.TransactionClient,
  agentSubject: string,
): Promise<void> {
  const expired = await transaction.$queryRaw<ExpiredRow[]>`
    WITH candidate AS (
      SELECT
        job.id,
        request.id AS request_id,
        request.correlation_id,
        request.device_id
      FROM execution_jobs AS job
      JOIN support_requests AS request ON request.id = job.request_id
      WHERE job.status = 'RUNNING'
        AND job.attempt_count >= ${maximumAttempts}
        AND job.lease_expires_at < clock_timestamp()
      ORDER BY job.lease_expires_at, job.id
      FOR UPDATE OF job SKIP LOCKED
      LIMIT 10
    )
    UPDATE execution_jobs AS job
    SET
      status = 'FAILED',
      claimed_at = NULL,
      claimed_by = NULL,
      claim_token_hash = NULL,
      lease_expires_at = NULL,
      result_idempotency_key = 'lease-expired:' || job.id::text,
      result_payload_hash = ${hashValue("lease-expired")},
      completed_at = clock_timestamp()
    FROM candidate
    WHERE job.id = candidate.id
    RETURNING
      job.id AS job_id,
      candidate.request_id,
      candidate.correlation_id,
      candidate.device_id
  `;

  for (const row of expired) {
    await transaction.supportRequest.update({
      where: { id: row.request_id },
      data: { status: "FAILED" },
    });
    const botCase = await transitionBotCase(
      transaction,
      row.request_id,
      "failed",
    );
    await transaction.$executeRaw`
      INSERT INTO execution_evidence (
        id,
        job_id,
        sequence,
        code,
        summary,
        recorded_at,
        created_at
      )
      VALUES (
        ${randomUUID()}::uuid,
        ${row.job_id}::uuid,
        1,
        'job.simulation.failed',
        ${evidenceSummaries["job.simulation.failed"]},
        clock_timestamp(),
        clock_timestamp()
      )
    `;
    await transaction.auditEvent.create({
      data: {
        id: randomUUID(),
        correlationId: row.correlation_id,
        actorSubject: agentSubject,
        eventType: "execution-job.lease-exhausted",
        entityType: "execution-job",
        entityId: row.job_id,
        payload: {
          jobId: row.job_id,
          requestId: row.request_id,
          deviceId: row.device_id,
        },
      },
    });
    await transaction.auditEvent.create({
      data: {
        id: randomUUID(),
        correlationId: row.correlation_id,
        actorSubject: agentSubject,
        eventType: "bot-case.result-recorded",
        entityType: "bot-case",
        entityId: botCase.id,
        payload: {
          caseId: botCase.id,
          requestId: row.request_id,
          result: "failed",
          status: "escalated",
        },
      },
    });
    await insertResultOutbox(transaction, {
      version: 1,
      requestId: row.request_id,
      jobId: row.job_id,
      correlationId: row.correlation_id,
      deviceId: row.device_id,
      result: "failed",
    });
  }
}

async function transitionBotCase(
  transaction: Prisma.TransactionClient,
  requestId: string,
  result: "succeeded" | "failed",
): Promise<{ readonly id: string }> {
  const rows =
    result === "succeeded"
      ? await transaction.$queryRaw<Array<{ id: string }>>`
          UPDATE bot_cases
          SET
            status = 'ATTENDED_WAITING_USER',
            result = 'SUCCEEDED',
            waiting_for_user_since = clock_timestamp(),
            escalated_at = NULL
          WHERE request_id = ${requestId}::uuid
          RETURNING id
        `
      : await transaction.$queryRaw<Array<{ id: string }>>`
          UPDATE bot_cases
          SET
            status = 'ESCALATED',
            result = 'FAILED',
            waiting_for_user_since = NULL,
            escalated_at = clock_timestamp()
          WHERE request_id = ${requestId}::uuid
          RETURNING id
        `;
  const botCase = rows[0];
  if (botCase === undefined) {
    throw new Error("bot_case_missing");
  }

  return botCase;
}

async function insertResultOutbox(
  transaction: Prisma.TransactionClient,
  event: ExecutionJobResultRecordedEventV1,
): Promise<void> {
  await transaction.$executeRaw`
    INSERT INTO outbox_events (
      id,
      aggregate_type,
      aggregate_id,
      event_type,
      payload,
      status,
      attempt_count,
      available_at,
      created_at
    )
    VALUES (
      ${randomUUID()}::uuid,
      'execution-job',
      ${event.jobId},
      ${executionJobResultRecordedEventType},
      ${JSON.stringify(event)}::jsonb,
      'PENDING',
      0,
      clock_timestamp(),
      clock_timestamp()
    )
  `;
}

function hashResult(result: ReportAgentJobResultRequest): string {
  return hashValue(
    JSON.stringify({
      evidence: result.evidence,
      result: result.result,
    }),
  );
}

function hashValue(value: string): string {
  return createHash("sha256").update(value, "utf8").digest("hex");
}

function invalidClaim(): ApplicationError {
  return new ApplicationError(
    "agent_claim_invalid",
    409,
    "The agent claim is invalid or expired.",
  );
}
