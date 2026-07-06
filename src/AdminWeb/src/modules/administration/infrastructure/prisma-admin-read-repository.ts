import type { PrismaClient } from "../../../generated/prisma/client";
import type {
  AdminAuditSummary,
  AdminLabReadModel,
  AdminOperationSummary,
  AdminReadRepository,
} from "../application/admin-read-repository";
import type { AdminLabHealthProvider } from "../application/admin-lab-health-provider";
import {
  createFakeTicketingHealth,
  LocalLabHealthProvider,
} from "./local-lab-health-provider";

export class PrismaAdminReadRepository implements AdminReadRepository {
  public constructor(
    private readonly prisma: PrismaClient,
    private readonly labHealthProvider: AdminLabHealthProvider =
      new LocalLabHealthProvider(),
  ) {}

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

  public async getLabReadModel(limit: number): Promise<AdminLabReadModel> {
    const checkedAt = new Date();
    const [
      requestCount,
      jobCount,
      caseCount,
      ticketCount,
      auditCount,
      outboxCount,
      recentOperations,
      recentAuditEvents,
      recentOutboxEvents,
      recentExternalTickets,
      connectorStatuses,
    ] = await Promise.all([
      this.prisma.supportRequest.count(),
      this.prisma.executionJob.count(),
      this.prisma.botCase.count(),
      this.prisma.externalTicket.count(),
      this.prisma.auditEvent.count(),
      this.prisma.outboxEvent.count(),
      this.listRecentOperations(limit),
      this.listRecentAuditEvents(limit),
      this.prisma.outboxEvent.findMany({
        orderBy: [{ createdAt: "desc" }, { id: "desc" }],
        take: limit,
        select: {
          id: true,
          eventType: true,
          aggregateType: true,
          status: true,
          attemptCount: true,
          createdAt: true,
        },
      }),
      this.prisma.externalTicket.findMany({
        orderBy: [{ createdAt: "desc" }, { id: "desc" }],
        take: limit,
        select: {
          id: true,
          externalReference: true,
          provider: true,
          status: true,
          reasonCode: true,
          correlationId: true,
          createdAt: true,
        },
      }),
      this.labHealthProvider.listComponentStatuses(),
    ]);

    return {
      components: [
        {
          id: "admin-web",
          name: "AdminWeb local",
          status: "available",
          source: "local",
          scope: "development",
          mode: "read-only",
          detail: "La ruta server-rendered respondio con identidad local.",
          lastCheckedAt: checkedAt,
        },
        {
          id: "postgresql",
          name: "PostgreSQL local",
          status: "available",
          source: "lab-real-sanitized",
          scope: "development",
          mode: "read-only",
          detail: "Lectura bounded completada sobre tablas locales existentes.",
          lastCheckedAt: checkedAt,
        },
        {
          id: "worker",
          name: "Worker Node",
          status: "not_checked",
          source: "local",
          scope: "local-demo",
          mode: "not-configured",
          detail: "Proceso durable separado; esta vista no inspecciona procesos.",
          lastCheckedAt: null,
        },
        ...connectorStatuses,
        createFakeTicketingHealth(ticketCount, checkedAt),
        {
          id: "windows-vm",
          name: "VM Windows 11",
          status: "not_checked",
          source: "local",
          scope: "local-demo",
          mode: "not-configured",
          detail: "La VM se inicia manualmente; el portal no administra Hyper-V.",
          lastCheckedAt: null,
        },
      ],
      metrics: [
        {
          label: "Solicitudes",
          value: requestCount,
          source: "lab-real-sanitized",
          detail: "Filas reales de support_requests en PostgreSQL local.",
        },
        {
          label: "Trabajos",
          value: jobCount,
          source: "lab-real-sanitized",
          detail: "Filas reales de execution_jobs en PostgreSQL local.",
        },
        {
          label: "Casos",
          value: caseCount,
          source: "lab-real-sanitized",
          detail: "Filas reales de bot_cases en PostgreSQL local.",
        },
        {
          label: "Tickets fake",
          value: ticketCount,
          source: "fake",
          detail: "Registros fake persistidos localmente; no son OpenText.",
        },
        {
          label: "Auditoria",
          value: auditCount,
          source: "lab-real-sanitized",
          detail: "Eventos append-only sin exponer payload.",
        },
        {
          label: "Outbox",
          value: outboxCount,
          source: "lab-real-sanitized",
          detail: "Eventos locales de outbox sin exponer payload.",
        },
      ],
      recentOperations,
      recentAuditEvents,
      recentOutboxEvents: recentOutboxEvents.map((event) => ({
        id: event.id,
        eventType: event.eventType,
        aggregateType: event.aggregateType,
        status: event.status,
        attemptCount: event.attemptCount,
        createdAt: event.createdAt,
      })),
      recentExternalTickets: recentExternalTickets.map((ticket) => ({
        id: ticket.id,
        reference: ticket.externalReference,
        provider: ticket.provider,
        status: ticket.status,
        reasonCode: ticket.reasonCode,
        correlationId: ticket.correlationId,
        createdAt: ticket.createdAt,
      })),
      boundaries: [
        "Solo Development y local-demo.",
        "Sin datos corporativos, tenants, endpoints ni secretos.",
        "Sin payloads de auditoria, outbox o prompts completos.",
        "Sin llamadas directas al DeviceAgent.",
        "Sin arranque o administracion de VM, servicios o bridges.",
        "Validate-only no equivale a despliegue enviado.",
      ],
    };
  }
}
