import type { PrismaClient } from "../../../generated/prisma/client";
import type {
  AdminAuditSummary,
  AdminLabReadModel,
  AdminLabStatus,
  AdminLabTrace,
  AdminLabTraceOutboxEvent,
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
      traces,
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
      this.listLabTraces(limit),
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
      traces,
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
        "Una consulta administrativa no crea solicitudes; solo POST confirmado crea solicitud, trabajo y caso.",
        "La evidencia mostrada esta saneada y no contiene comandos generados por IA.",
      ],
    };
  }

  private async listLabTraces(limit: number): Promise<readonly AdminLabTrace[]> {
    const requests = await this.prisma.supportRequest.findMany({
      orderBy: [{ createdAt: "desc" }, { id: "desc" }],
      take: limit,
      select: {
        id: true,
        reference: true,
        idempotencyKey: true,
        correlationId: true,
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
            id: true,
            status: true,
            attemptCount: true,
            resultIdempotencyKey: true,
            completedAt: true,
            evidence: {
              orderBy: [{ sequence: "asc" }],
              select: {
                code: true,
                summary: true,
                recordedAt: true,
              },
            },
          },
        },
        botCase: {
          select: {
            id: true,
            status: true,
            result: true,
            waitingForUserSince: true,
            escalatedAt: true,
            externalTicket: {
              select: {
                externalReference: true,
                provider: true,
                createdAt: true,
              },
            },
          },
        },
      },
    });

    if (requests.length === 0) {
      return [];
    }

    const aggregateFilters = requests.flatMap((request) => {
      const filters = [
        {
          aggregateType: "support-request",
          aggregateId: request.id,
        },
      ];

      if (request.job !== null) {
        filters.push({
          aggregateType: "execution-job",
          aggregateId: request.job.id,
        });
      }

      if (request.botCase !== null) {
        filters.push({
          aggregateType: "bot-case",
          aggregateId: request.botCase.id,
        });
      }

      return filters;
    });

    const [outboxEvents, duplicateRequestRows, duplicateResultRows] =
      await Promise.all([
        this.prisma.outboxEvent.findMany({
          where: { OR: aggregateFilters },
          orderBy: [{ createdAt: "asc" }, { id: "asc" }],
          select: {
            aggregateType: true,
            aggregateId: true,
            eventType: true,
            status: true,
            attemptCount: true,
            createdAt: true,
            syntheticEffect: {
              select: {
                effectType: true,
              },
            },
          },
        }),
        Promise.all(
          requests.map((request) =>
            this.prisma.supportRequest.count({
              where: { idempotencyKey: request.idempotencyKey },
            }),
          ),
        ),
        Promise.all(
          requests.map((request) => {
            const resultIdempotencyKey = request.job?.resultIdempotencyKey;
            return resultIdempotencyKey === null ||
              resultIdempotencyKey === undefined
              ? Promise.resolve(0)
              : this.prisma.executionJob.count({
                  where: { resultIdempotencyKey },
                });
          }),
        ),
      ]);

    const outboxByAggregate = new Map<string, AdminLabTraceOutboxEvent[]>();
    for (const event of outboxEvents) {
      const key = aggregateKey(event.aggregateType, event.aggregateId);
      const existing = outboxByAggregate.get(key) ?? [];
      existing.push({
        eventType: event.eventType,
        aggregateType: event.aggregateType,
        status: event.status,
        attemptCount: event.attemptCount,
        effectType: event.syntheticEffect?.effectType ?? null,
        createdAt: event.createdAt,
      });
      outboxByAggregate.set(key, existing);
    }

    return requests.map((request, index) => {
      const requestOutbox =
        outboxByAggregate.get(aggregateKey("support-request", request.id)) ?? [];
      const jobOutbox =
        request.job === null
          ? []
          : (outboxByAggregate.get(
              aggregateKey("execution-job", request.job.id),
            ) ?? []);
      const caseOutbox =
        request.botCase === null
          ? []
          : (outboxByAggregate.get(
              aggregateKey("bot-case", request.botCase.id),
            ) ?? []);
      const allOutbox = [...requestOutbox, ...jobOutbox, ...caseOutbox];

      return {
        requestId: request.id,
        requestReference: request.reference,
        correlationId: request.correlationId,
        deviceName: request.device.displayName,
        productName: `${request.productId} ${request.productVersion}`,
        requestStatus: request.status,
        jobId: request.job?.id ?? null,
        jobStatus: request.job?.status ?? null,
        caseId: request.botCase?.id ?? null,
        caseStatus: request.botCase?.status ?? null,
        ticketReference:
          request.botCase?.externalTicket?.externalReference ?? null,
        createdAt: request.createdAt,
        stages: createTraceStages(request, requestOutbox, jobOutbox, caseOutbox),
        evidence:
          request.job?.evidence.map((evidence) => ({
            code: evidence.code,
            summary: evidence.summary,
            recordedAt: evidence.recordedAt,
          })) ?? [],
        outboxEvents: allOutbox,
        idempotency: {
          requestReplayProtected: true,
          resultReplayProtected: request.job?.resultIdempotencyKey != null,
          duplicateRequestRows: duplicateRequestRows[index] ?? 0,
          duplicateResultRows: duplicateResultRows[index] ?? 0,
        },
      };
    });
  }
}

function createTraceStages(
  request: {
    readonly id: string;
    readonly createdAt: Date;
    readonly status: string;
    readonly job: {
      readonly status: string;
      readonly attemptCount: number;
      readonly completedAt: Date | null;
      readonly evidence: readonly { readonly recordedAt: Date }[];
    } | null;
    readonly botCase: {
      readonly status: string;
      readonly result: string;
      readonly waitingForUserSince: Date | null;
      readonly escalatedAt: Date | null;
      readonly externalTicket: {
        readonly provider: string;
        readonly createdAt: Date;
      } | null;
    } | null;
  },
  requestOutbox: readonly AdminLabTraceOutboxEvent[],
  jobOutbox: readonly AdminLabTraceOutboxEvent[],
  caseOutbox: readonly AdminLabTraceOutboxEvent[],
): AdminLabTrace["stages"] {
  const dispatchReady = requestOutbox.some(
    (event) =>
      event.status === "COMPLETED" &&
      event.effectType === "support-request.dispatch-ready.v1",
  );
  const resultRecorded = jobOutbox.some(
    (event) => event.status === "COMPLETED",
  );
  const ticketCreated = caseOutbox.some(
    (event) =>
      event.status === "COMPLETED" &&
      event.effectType === "bot-case.external-ticket-created.v1",
  );

  return [
    {
      id: "winui-confirmation",
      label: "WinUI confirmacion explicita",
      status: "available",
      source: "lab-real-sanitized",
      detail:
        "La fila existe porque una confirmacion explicita creo solicitud, trabajo y caso; las consultas no crean solicitudes.",
      recordedAt: request.createdAt,
    },
    {
      id: "api-local",
      label: "API local",
      status: "available",
      source: "lab-real-sanitized",
      detail: `Solicitud ${request.status.toLocaleLowerCase("es-MX")} persistida en transaccion local.`,
      recordedAt: request.createdAt,
    },
    {
      id: "worker",
      label: "Worker y outbox",
      status: dispatchReady ? "available" : "not_checked",
      source: "lab-real-sanitized",
      detail: dispatchReady
        ? "Outbox completo genero efecto dispatch-ready sin exponer payload."
        : "Pendiente o no comprobado; la vista no arranca el worker.",
      recordedAt: requestOutbox.at(-1)?.createdAt ?? null,
    },
    {
      id: "agent",
      label: "Agente simulado o VM",
      status: mapJobStageStatus(request.job?.status ?? null),
      source: "local",
      detail:
        request.job === null
          ? "Sin trabajo asociado."
          : `Trabajo ${request.job.status.toLocaleLowerCase("es-MX")} con ${request.job.attemptCount} intento(s); sin comandos generados por IA.`,
      recordedAt:
        request.job?.completedAt ??
        request.job?.evidence.at(-1)?.recordedAt ??
        null,
    },
    {
      id: "evidence",
      label: "Evidencia saneada",
      status:
        request.job !== null && request.job.evidence.length > 0
          ? "available"
          : "not_checked",
      source: "lab-real-sanitized",
      detail:
        request.job !== null && request.job.evidence.length > 0
          ? `${request.job.evidence.length} evidencia(s) con resumen acotado.`
          : "Sin evidencia registrada todavia.",
      recordedAt: request.job?.evidence.at(-1)?.recordedAt ?? null,
    },
    {
      id: "case",
      label: "Caso",
      status: request.botCase === null ? "not_checked" : "available",
      source: "lab-real-sanitized",
      detail:
        request.botCase === null
          ? "Sin caso asociado."
          : `Caso ${request.botCase.status.toLocaleLowerCase("es-MX")} con resultado ${request.botCase.result.toLocaleLowerCase("es-MX")}.`,
      recordedAt:
        request.botCase?.waitingForUserSince ??
        request.botCase?.escalatedAt ??
        null,
    },
    {
      id: "ticket",
      label: "Ticket fake",
      status:
        request.botCase?.externalTicket === null ||
        request.botCase?.externalTicket === undefined
          ? resultRecorded
            ? "not_checked"
            : "validate-only"
          : ticketCreated
            ? "available"
            : "not_checked",
      source: "fake",
      detail:
        request.botCase?.externalTicket === null ||
        request.botCase?.externalTicket === undefined
          ? "Sin ticket fake; puede ser exito atendido o outbox pendiente."
          : `Ticket ${request.botCase.externalTicket.provider.toLocaleLowerCase("es-MX")} persistido localmente.`,
      recordedAt: request.botCase?.externalTicket?.createdAt ?? null,
    },
  ];
}

function mapJobStageStatus(status: string | null): AdminLabStatus {
  if (status === null) {
    return "not_checked";
  }

  if (status === "COMPLETED" || status === "FAILED") {
    return "available";
  }

  if (status === "RUNNING") {
    return "validate-only";
  }

  return "not_checked";
}

function aggregateKey(aggregateType: string, aggregateId: string): string {
  return `${aggregateType}:${aggregateId}`;
}
