import {
  supportRequestConfirmedEventType,
  type SupportRequestConfirmedEventV1,
  type SupportRequestView,
} from "@it-support-native/control-plane-contracts";
import { randomUUID } from "node:crypto";
import {
  Prisma,
  type PrismaClient,
} from "../../../generated/prisma/client";
import { ApplicationError } from "../../../platform/errors/application-error";
import type {
  CreateConfirmedInstallationCommand,
  CreatedSupportRequest,
} from "../domain/support-request";
import type { SupportRequestRepository } from "../application/support-request-repository";

type RequestWithJob = Prisma.SupportRequestGetPayload<{
  include: { job: true };
}>;

export class PrismaSupportRequestRepository
  implements SupportRequestRepository
{
  public constructor(private readonly prisma: PrismaClient) {}

  public async createConfirmedInstallation(
    command: CreateConfirmedInstallationCommand,
    payloadHash: string,
  ): Promise<CreatedSupportRequest> {
    return this.prisma.$transaction(
      async (transaction) => {
        await transaction.$queryRaw`
          SELECT
            pg_advisory_xact_lock(
              hashtextextended(${command.idempotencyKey}, 0)
            ) IS NULL AS acquired
        `;

        const existing = await transaction.supportRequest.findUnique({
          where: { idempotencyKey: command.idempotencyKey },
          include: { job: true },
        });

        if (existing !== null) {
          if (existing.idempotencyHash !== payloadHash) {
            throw new ApplicationError(
              "idempotency_conflict",
              409,
              "The idempotency key was already used for another request.",
            );
          }

          return {
            request: mapRequest(existing),
            replayed: true,
          };
        }

        const device = await transaction.device.findUnique({
          where: { id: command.deviceId },
          select: { id: true },
        });

        if (device === null) {
          throw new ApplicationError(
            "device_not_found",
            404,
            "The target device was not found.",
          );
        }

        const requestId = randomUUID();
        const jobId = randomUUID();
        const outboxId = randomUUID();
        const reference = `REQ-${requestId}`;
        const event: SupportRequestConfirmedEventV1 = {
          version: 1,
          requestId,
          jobId,
          correlationId: command.correlationId,
          actorSubject: command.actorSubject,
          deviceId: command.deviceId,
          actionId: command.actionId,
          productId: command.productId,
          productVersion: command.productVersion,
        };

        const created = await transaction.supportRequest.create({
          data: {
            id: requestId,
            reference,
            idempotencyKey: command.idempotencyKey,
            idempotencyHash: payloadHash,
            correlationId: command.correlationId,
            requesterSubject: command.actorSubject,
            deviceId: command.deviceId,
            productId: command.productId,
            productVersion: command.productVersion,
            actionId: command.actionId,
            job: {
              create: {
                id: jobId,
                actionId: command.actionId,
                targetId: command.productId,
                targetVersion: command.productVersion,
              },
            },
          },
          include: { job: true },
        });

        await transaction.auditEvent.create({
          data: {
            id: randomUUID(),
            correlationId: command.correlationId,
            actorSubject: command.actorSubject,
            eventType: "support-request.confirmed",
            entityType: "support-request",
            entityId: requestId,
            payload: {
              requestId,
              jobId,
              deviceId: command.deviceId,
              actionId: command.actionId,
              productId: command.productId,
              productVersion: command.productVersion,
            },
          },
        });

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
            ${outboxId}::uuid,
            'support-request',
            ${requestId},
            ${supportRequestConfirmedEventType},
            ${JSON.stringify(event)}::jsonb,
            'PENDING',
            0,
            clock_timestamp(),
            clock_timestamp()
          )
        `;

        return {
          request: mapRequest(created),
          replayed: false,
        };
      },
      {
        isolationLevel: Prisma.TransactionIsolationLevel.Serializable,
        maxWait: 5_000,
        timeout: 10_000,
      },
    );
  }

  public async findById(requestId: string): Promise<SupportRequestView | null> {
    const request = await this.prisma.supportRequest.findUnique({
      where: { id: requestId },
      include: { job: true },
    });

    return request === null ? null : mapRequest(request);
  }
}

function mapRequest(request: RequestWithJob): SupportRequestView {
  if (request.job === null) {
    throw new Error("support_request_job_missing");
  }

  return {
    id: request.id,
    reference: request.reference,
    correlationId: request.correlationId,
    status: mapRequestStatus(request.status),
    deviceId: request.deviceId,
    productId: request.productId,
    productVersion: request.productVersion,
    actionId: request.actionId,
    createdAt: request.createdAt.toISOString(),
    job: {
      id: request.job.id,
      status: mapJobStatus(request.job.status),
    },
  };
}

function mapRequestStatus(
  status: "CONFIRMED" | "COMPLETED" | "FAILED",
): SupportRequestView["status"] {
  return status.toLowerCase() as SupportRequestView["status"];
}

function mapJobStatus(
  status: "QUEUED" | "COMPLETED" | "FAILED",
): SupportRequestView["job"]["status"] {
  return status.toLowerCase() as SupportRequestView["job"]["status"];
}
