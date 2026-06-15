import type { PrismaClient } from "../../../generated/prisma/client";
import type {
  AdminAuditSummary,
  AdminOperationSummary,
  AdminReadRepository,
} from "../application/admin-read-repository";

export class PrismaAdminReadRepository implements AdminReadRepository {
  public constructor(private readonly prisma: PrismaClient) {}

  public async listRecentOperations(
    limit: number,
  ): Promise<readonly AdminOperationSummary[]> {
    const requests = await this.prisma.supportRequest.findMany({
      orderBy: [{ createdAt: "desc" }, { id: "desc" }],
      take: limit,
      select: {
        id: true,
        reference: true,
        productId: true,
        productVersion: true,
        status: true,
        createdAt: true,
        device: {
          select: {
            displayName: true,
          },
        },
        job: {
          select: {
            status: true,
          },
        },
        botCase: {
          select: {
            status: true,
            externalTicket: {
              select: {
                externalReference: true,
              },
            },
          },
        },
      },
    });

    return requests.map((request) => ({
      id: request.id,
      reference: request.reference,
      deviceName: request.device.displayName,
      productName: `${request.productId} ${request.productVersion}`,
      requestStatus: request.status,
      jobStatus: request.job?.status ?? null,
      caseStatus: request.botCase?.status ?? null,
      ticketReference:
        request.botCase?.externalTicket?.externalReference ?? null,
      createdAt: request.createdAt,
    }));
  }

  public async listRecentAuditEvents(
    limit: number,
  ): Promise<readonly AdminAuditSummary[]> {
    return this.prisma.auditEvent.findMany({
      orderBy: [{ createdAt: "desc" }, { id: "desc" }],
      take: limit,
      select: {
        id: true,
        eventType: true,
        entityType: true,
        entityId: true,
        actorSubject: true,
        correlationId: true,
        createdAt: true,
      },
    });
  }
}
