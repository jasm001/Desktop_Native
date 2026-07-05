import { beforeAll, afterAll, describe, expect, it } from "vitest";
import {
  apiErrorEnvelopeSchema,
  botCaseEscalationRequestedEventType,
  botCaseEscalationRequestedEventV1Schema,
  catalogResponseSchema,
  claimAgentJobResponseSchema,
  createSoftwareInstallationResponseSchema,
  getBotCaseResponseSchema,
  getSupportRequestResponseSchema,
  reportAgentJobResultResponseSchema,
} from "@it-support-native/control-plane-contracts";
import { POST as claimAgentJobRoute } from "@/app/api/v1/agent/jobs/claim/route";
import { POST as reportAgentJobResultRoute } from "@/app/api/v1/agent/jobs/[jobId]/result/route";
import { GET as getCatalog } from "@/app/api/v1/catalog/products/route";
import { GET as getBotCaseRoute } from "@/app/api/v1/requests/[requestId]/case/route";
import { GET as getRequestRoute } from "@/app/api/v1/requests/[requestId]/route";
import { POST as createRequestRoute } from "@/app/api/v1/requests/software-installations/route";
import { listCatalogProducts } from "@/modules/catalog/application/list-catalog-products";
import {
  getRecentAdminAuditEvents,
  getRecentAdminOperations,
} from "@/modules/administration/application/get-admin-read-model";
import { getAdminLabReadModel } from "@/modules/administration/application/get-admin-lab-read-model";
import { PrismaAdminReadRepository } from "@/modules/administration/infrastructure/prisma-admin-read-repository";
import { CreateConfirmedInstallation } from "@/modules/requests/application/create-confirmed-installation";
import { GetSupportRequest } from "@/modules/requests/application/get-support-request";
import { PrismaSupportRequestRepository } from "@/modules/requests/infrastructure/prisma-support-request-repository";
import { disconnectPrisma, getPrisma } from "@/platform/db/prisma";
import type { ApplicationError } from "@/platform/errors/application-error";

const principal = {
  subject: "development-user-001",
  displayName: "Synthetic Development User",
  role: "DeveloperAllAccess",
} as const;

const input = {
  confirmed: true,
  deviceId: "local-device-001",
  productId: "secure-transfer",
  productVersion: "6.5",
  actionId: "software.install.simulated.v1",
} as const;

describe("control plane persistence", () => {
  const prisma = getPrisma();
  const repository = new PrismaSupportRequestRepository(prisma);
  const createRequest = new CreateConfirmedInstallation(repository);
  const getRequest = new GetSupportRequest(repository);

  beforeAll(async () => {
    await prisma.syntheticOutboxEffect.deleteMany();
    await prisma.outboxEvent.deleteMany();
    await prisma.auditEvent.deleteMany();
    await prisma.executionEvidence.deleteMany();
    await prisma.executionJob.deleteMany();
    await prisma.externalTicket.deleteMany();
    await prisma.botCase.deleteMany();
    await prisma.supportRequest.deleteMany();
    await prisma.device.upsert({
      where: { id: input.deviceId },
      update: {
        displayName: "Synthetic Windows 11 device",
        environment: "development",
      },
      create: {
        id: input.deviceId,
        displayName: "Synthetic Windows 11 device",
        environment: "development",
      },
    });
  });

  afterAll(async () => {
    await disconnectPrisma();
  });

  it("persists request, job, case, audit, and outbox atomically", async () => {
    const result = await createRequest.execute(
      input,
      "integration-key-1",
      "integration-correlation-1",
      principal,
    );

    expect(result.replayed).toBe(false);
    expect(result.request.status).toBe("confirmed");
    expect(result.request.job.status).toBe("queued");
    await expect(prisma.supportRequest.count()).resolves.toBe(1);
    await expect(prisma.executionJob.count()).resolves.toBe(1);
    await expect(prisma.botCase.count()).resolves.toBe(1);
    await expect(prisma.auditEvent.count()).resolves.toBe(2);
    await expect(prisma.outboxEvent.count()).resolves.toBe(1);
    await expect(
      prisma.botCase.findUniqueOrThrow({
        where: { requestId: result.request.id },
      }),
    ).resolves.toMatchObject({
      correlationId: "integration-correlation-1",
      category: "SOFTWARE_INSTALLATION",
      status: "OPEN",
      result: "PENDING",
    });
  });

  it("replays the same idempotency key without duplicate records", async () => {
    const replay = await createRequest.execute(
      input,
      "integration-key-1",
      "integration-correlation-retry",
      principal,
    );

    expect(replay.replayed).toBe(true);
    await expect(prisma.supportRequest.count()).resolves.toBe(1);
    await expect(prisma.executionJob.count()).resolves.toBe(1);
    await expect(prisma.botCase.count()).resolves.toBe(1);
    await expect(prisma.auditEvent.count()).resolves.toBe(2);
    await expect(prisma.outboxEvent.count()).resolves.toBe(1);
  });

  it("uses the PostgreSQL clock for persisted transaction timestamps", async () => {
    const timestamps = await prisma.$queryRaw<
      Array<{ source: string; delta_seconds: number }>
    >`
      SELECT
        source,
        abs(extract(epoch FROM persisted_at - clock_timestamp()))::float8
          AS delta_seconds
      FROM (
        SELECT 'devices.created_at' AS source, created_at AS persisted_at
        FROM devices
        WHERE id = ${input.deviceId}
        UNION ALL
        SELECT 'devices.updated_at', updated_at
        FROM devices
        WHERE id = ${input.deviceId}
        UNION ALL
        SELECT 'support_requests.created_at', created_at
        FROM support_requests
        UNION ALL
        SELECT 'support_requests.updated_at', updated_at FROM support_requests
        UNION ALL
        SELECT 'execution_jobs.created_at', created_at FROM execution_jobs
        UNION ALL
        SELECT 'execution_jobs.updated_at', updated_at FROM execution_jobs
        UNION ALL
        SELECT 'bot_cases.created_at', created_at FROM bot_cases
        UNION ALL
        SELECT 'bot_cases.updated_at', updated_at FROM bot_cases
        UNION ALL
        SELECT 'audit_events.created_at', created_at FROM audit_events
        UNION ALL
        SELECT 'outbox_events.created_at', created_at FROM outbox_events
        UNION ALL
        SELECT 'outbox_events.available_at', available_at FROM outbox_events
      ) AS persisted_timestamps
      ORDER BY source
    `;

    expect(timestamps).toHaveLength(12);
    expect(
      timestamps.filter((timestamp) => timestamp.delta_seconds >= 10),
    ).toEqual([]);
  });

  it("rejects reuse of an idempotency key with another payload", async () => {
    const conflictingInput = {
      ...input,
      deviceId: "local-device-002",
    };

    await expect(
      createRequest.execute(
        conflictingInput,
        "integration-key-1",
        "integration-correlation-conflict",
        principal,
      ),
    ).rejects.toMatchObject({
      code: "idempotency_conflict",
      httpStatus: 409,
    } satisfies Partial<ApplicationError>);
  });

  it("keeps catalog and status queries free of side effects", async () => {
    const existing = await prisma.supportRequest.findFirstOrThrow({
      select: { id: true },
    });
    const countsBefore = await mutationCounts();

    expect(listCatalogProducts(20).items.length).toBeGreaterThan(0);
    await expect(getRequest.execute(existing.id)).resolves.toMatchObject({
      id: existing.id,
    });

    await expect(mutationCounts()).resolves.toEqual(countsBefore);
  });

  it("serves bounded administrative read models without side effects", async () => {
    const adminRepository = new PrismaAdminReadRepository(prisma);
    const countsBefore = await mutationCounts();
    const [operations, auditEvents] = await Promise.all([
      getRecentAdminOperations(adminRepository),
      getRecentAdminAuditEvents(adminRepository),
    ]);

    expect(operations.length).toBeGreaterThan(0);
    expect(operations.length).toBeLessThanOrEqual(25);
    expect(operations[0]).toEqual(
      expect.objectContaining({
        reference: expect.any(String),
        deviceName: "Synthetic Windows 11 device",
        createdAt: expect.any(Date),
      }),
    );
    expect(auditEvents.length).toBeGreaterThan(0);
    expect(auditEvents.length).toBeLessThanOrEqual(25);
    expect(auditEvents[0]).toEqual(
      expect.objectContaining({
        eventType: expect.any(String),
        entityType: expect.any(String),
        correlationId: expect.any(String),
        createdAt: expect.any(Date),
      }),
    );
    expect(auditEvents[0]).not.toHaveProperty("payload");
    await expect(mutationCounts()).resolves.toEqual(countsBefore);
  });

  it("serves lab-real local read models without side effects", async () => {
    const adminRepository = new PrismaAdminReadRepository(prisma);
    const countsBefore = await mutationCounts();
    const lab = await getAdminLabReadModel(adminRepository);

    expect(lab.components).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          id: "postgresql",
          status: "available",
          source: "lab-real-sanitized",
        }),
        expect.objectContaining({
          id: "windows-vm",
          status: "not_checked",
        }),
      ]),
    );
    expect(lab.metrics).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          label: "Solicitudes",
          value: countsBefore.requests,
          source: "lab-real-sanitized",
        }),
        expect.objectContaining({
          label: "Outbox",
          value: countsBefore.outbox,
        }),
      ]),
    );
    expect(lab.recentOperations.length).toBeGreaterThan(0);
    expect(lab.recentAuditEvents.length).toBeGreaterThan(0);
    expect(lab.recentOutboxEvents.length).toBeGreaterThan(0);
    expect(lab.recentOutboxEvents[0]).not.toHaveProperty("payload");
    expect(lab.recentExternalTickets).toEqual([]);
    expect(JSON.stringify(lab)).not.toContain("api-key");
    await expect(mutationCounts()).resolves.toEqual(countsBefore);
  });

  it("rejects updates and deletes against append-only audit events", async () => {
    const audit = await prisma.auditEvent.findFirstOrThrow({
      select: { id: true },
    });

    await expect(
      prisma.auditEvent.update({
        where: { id: audit.id },
        data: { eventType: "tampered" },
      }),
    ).rejects.toThrow();
    await expect(
      prisma.auditEvent.delete({
        where: { id: audit.id },
      }),
    ).rejects.toThrow();
    await expect(prisma.auditEvent.count()).resolves.toBe(2);
  });

  it("serves validated HTTP v1 envelopes without duplicate mutations", async () => {
    const existing = await prisma.supportRequest.findFirstOrThrow({
      select: { id: true },
    });
    const countsBefore = await mutationCounts();
    const createResponse = await createRequestRoute(
      new Request(
        "http://localhost/api/v1/requests/software-installations",
        {
          method: "POST",
          headers: {
            "content-type": "application/json",
            "idempotency-key": "integration-key-1",
            "x-correlation-id": "http-contract-correlation",
          },
          body: JSON.stringify(input),
        },
      ),
    );
    const statusResponse = await getRequestRoute(
      new Request(
        `http://localhost/api/v1/requests/${existing.id}`,
        {
          headers: {
            "x-correlation-id": "http-status-correlation",
          },
        },
      ),
      {
        params: Promise.resolve({ requestId: existing.id }),
      },
    );
    const caseResponse = await getBotCaseRoute(
      new Request(
        `http://localhost/api/v1/requests/${existing.id}/case`,
        {
          headers: {
            "x-correlation-id": "http-case-correlation",
          },
        },
      ),
      {
        params: Promise.resolve({ requestId: existing.id }),
      },
    );
    const catalogResponse = getCatalog(
      new Request("http://localhost/api/v1/catalog/products?limit=1", {
        headers: {
          "x-correlation-id": "http-catalog-correlation",
        },
      }),
    );

    expect(createResponse.status).toBe(200);
    expect(
      createSoftwareInstallationResponseSchema.parse(
        await createResponse.json(),
      ).data.replayed,
    ).toBe(true);
    expect(statusResponse.status).toBe(200);
    expect(
      getSupportRequestResponseSchema.parse(await statusResponse.json()).data
        .request.id,
    ).toBe(existing.id);
    expect(caseResponse.status).toBe(200);
    expect(
      getBotCaseResponseSchema.parse(await caseResponse.json()).data.case,
    ).toMatchObject({
      requestId: existing.id,
      status: "open",
      result: "pending",
      externalTicket: null,
    });
    expect(catalogResponse.status).toBe(200);
    expect(
      catalogResponseSchema.parse(await catalogResponse.json()).data.items,
    ).toHaveLength(1);
    await expect(mutationCounts()).resolves.toEqual(countsBefore);
  });

  it("requires worker dispatch before an agent can claim the job", async () => {
    const response = await claimAgentJobRoute(
      agentRequest("/api/v1/agent/jobs/claim", {
        deviceId: input.deviceId,
      }),
    );

    expect(response.status).toBe(200);
    expect(
      claimAgentJobResponseSchema.parse(await response.json()).data.job,
    ).toBeNull();
  });

  it("claims and records an idempotent sanitized agent result", async () => {
    const outbox = await prisma.outboxEvent.findFirstOrThrow({
      select: { id: true },
    });
    await prisma.syntheticOutboxEffect.create({
      data: {
        id: crypto.randomUUID(),
        outboxEventId: outbox.id,
        effectType: "support-request.dispatch-ready.v1",
        payload: { result: "dispatch_ready" },
      },
    });
    await prisma.outboxEvent.update({
      where: { id: outbox.id },
      data: {
        status: "COMPLETED",
        completedAt: new Date(),
      },
    });

    const claimResponse = await claimAgentJobRoute(
      agentRequest("/api/v1/agent/jobs/claim", {
        deviceId: input.deviceId,
      }),
    );
    const claim = claimAgentJobResponseSchema.parse(
      await claimResponse.json(),
    ).data.job;
    expect(claimResponse.status).toBe(200);
    expect(claim).not.toBeNull();
    if (claim === null) {
      throw new Error("agent_claim_missing");
    }

    const recordedAt = new Date().toISOString();
    const resultBody = {
      claimToken: claim.claimToken,
      result: "succeeded",
      evidence: [
        { code: "job.accepted", recordedAt },
        { code: "job.simulation.started", recordedAt },
        { code: "job.simulation.verified", recordedAt },
      ],
    } as const;
    const resultUrl = `/api/v1/agent/jobs/${claim.jobId}/result`;
    const firstResponse = await reportAgentJobResultRoute(
      agentRequest(resultUrl, resultBody, "agent-result-key-1"),
      { params: Promise.resolve({ jobId: claim.jobId }) },
    );
    const replayResponse = await reportAgentJobResultRoute(
      agentRequest(resultUrl, resultBody, "agent-result-key-1"),
      { params: Promise.resolve({ jobId: claim.jobId }) },
    );
    const conflictResponse = await reportAgentJobResultRoute(
      agentRequest(
        resultUrl,
        {
          claimToken: claim.claimToken,
          result: "failed",
          evidence: [{ code: "job.simulation.failed", recordedAt }],
        },
        "agent-result-key-1",
      ),
      { params: Promise.resolve({ jobId: claim.jobId }) },
    );
    const first = reportAgentJobResultResponseSchema.parse(
      await firstResponse.json(),
    );
    const replay = reportAgentJobResultResponseSchema.parse(
      await replayResponse.json(),
    );

    expect(firstResponse.status).toBe(201);
    expect(first.data.replayed).toBe(false);
    expect(first.data.request.status).toBe("completed");
    expect(first.data.request.job.status).toBe("completed");
    expect(first.data.request.job.evidence).toHaveLength(3);
    expect(replayResponse.status).toBe(200);
    expect(replay.data.replayed).toBe(true);
    expect(conflictResponse.status).toBe(409);
    expect(
      apiErrorEnvelopeSchema.parse(await conflictResponse.json()).error.code,
    ).toBe("idempotency_conflict");
    await expect(prisma.executionEvidence.count()).resolves.toBe(3);
    await expect(prisma.outboxEvent.count()).resolves.toBe(2);
    await expect(
      prisma.botCase.findUniqueOrThrow({
        where: { requestId: first.data.request.id },
      }),
    ).resolves.toMatchObject({
      status: "ATTENDED_WAITING_USER",
      result: "SUCCEEDED",
      escalatedAt: null,
    });
    const attendedCase = await prisma.botCase.findUniqueOrThrow({
      where: { requestId: first.data.request.id },
    });
    expect(attendedCase.waitingForUserSince).not.toBeNull();
  });

  it("escalates the case for an idempotent failed agent result", async () => {
    const created = await createRequest.execute(
      input,
      "integration-key-failed-result",
      "integration-correlation-failed-result",
      principal,
    );
    const outbox = await prisma.outboxEvent.findFirstOrThrow({
      where: {
        aggregateId: created.request.id,
        eventType: "support-request.confirmed.v1",
      },
      select: { id: true },
    });
    await prisma.syntheticOutboxEffect.create({
      data: {
        id: crypto.randomUUID(),
        outboxEventId: outbox.id,
        effectType: "support-request.dispatch-ready.v1",
        payload: { result: "dispatch_ready" },
      },
    });
    await prisma.outboxEvent.update({
      where: { id: outbox.id },
      data: {
        status: "COMPLETED",
        completedAt: new Date(),
      },
    });

    const claimResponse = await claimAgentJobRoute(
      agentRequest("/api/v1/agent/jobs/claim", {
        deviceId: input.deviceId,
      }),
    );
    const claim = claimAgentJobResponseSchema.parse(
      await claimResponse.json(),
    ).data.job;
    expect(claim).not.toBeNull();
    if (claim === null) {
      throw new Error("agent_claim_missing");
    }

    const resultBody = {
      claimToken: claim.claimToken,
      result: "failed",
      evidence: [
        {
          code: "job.simulation.failed",
          recordedAt: new Date().toISOString(),
        },
      ],
    } as const;
    const resultUrl = `/api/v1/agent/jobs/${claim.jobId}/result`;
    const firstResponse = await reportAgentJobResultRoute(
      agentRequest(resultUrl, resultBody, "agent-failed-result-key-1"),
      { params: Promise.resolve({ jobId: claim.jobId }) },
    );
    const replayResponse = await reportAgentJobResultRoute(
      agentRequest(resultUrl, resultBody, "agent-failed-result-key-1"),
      { params: Promise.resolve({ jobId: claim.jobId }) },
    );

    expect(firstResponse.status).toBe(201);
    expect(replayResponse.status).toBe(200);
    await expect(
      prisma.botCase.findUniqueOrThrow({
        where: { requestId: created.request.id },
      }),
    ).resolves.toMatchObject({
      status: "ESCALATED",
      result: "FAILED",
      waitingForUserSince: null,
    });
    await expect(
      prisma.botCase.count({ where: { requestId: created.request.id } }),
    ).resolves.toBe(1);
    const escalationEvents = await prisma.outboxEvent.findMany({
      where: {
        aggregateType: "bot-case",
        eventType: botCaseEscalationRequestedEventType,
        payload: {
          path: ["requestId"],
          equals: created.request.id,
        },
      },
    });
    expect(escalationEvents).toHaveLength(1);
    const escalation = botCaseEscalationRequestedEventV1Schema.parse(
      escalationEvents[0]?.payload,
    );
    expect(escalation).toMatchObject({
      requestId: created.request.id,
      jobId: claim.jobId,
      reasonCode: "execution_failed",
      productId: input.productId,
      productVersion: input.productVersion,
    });
    await expect(prisma.externalTicket.count()).resolves.toBe(0);

    const externalTicketId = crypto.randomUUID();
    await prisma.externalTicket.create({
      data: {
        id: externalTicketId,
        caseId: escalation.caseId,
        externalReference: `FAKE-${externalTicketId.replaceAll("-", "").slice(0, 20).toUpperCase()}`,
        category: "SOFTWARE_INSTALLATION",
        correlationId: escalation.correlationId,
        reasonCode: escalation.reasonCode,
        description:
          "Synthetic escalation for secure-transfer 6.5; human support review required.",
      },
    });
    const caseResponse = await getBotCaseRoute(
      new Request(
        `http://localhost/api/v1/requests/${created.request.id}/case`,
        { headers: { "x-correlation-id": "ticket-query-correlation" } },
      ),
      { params: Promise.resolve({ requestId: created.request.id }) },
    );
    expect(
      getBotCaseResponseSchema.parse(await caseResponse.json()).data.case
        .externalTicket,
    ).toMatchObject({
      id: externalTicketId,
      provider: "fake",
      category: "software_installation",
      status: "open",
      reasonCode: "execution_failed",
    });
  });

  it("fails a job after three expired agent claims", async () => {
    const created = await createRequest.execute(
      input,
      "integration-key-exhausted-claim",
      "integration-correlation-exhausted-claim",
      principal,
    );
    const outbox = await prisma.outboxEvent.findFirstOrThrow({
      where: {
        aggregateId: created.request.id,
        eventType: "support-request.confirmed.v1",
      },
      select: { id: true },
    });
    await prisma.syntheticOutboxEffect.create({
      data: {
        id: crypto.randomUUID(),
        outboxEventId: outbox.id,
        effectType: "support-request.dispatch-ready.v1",
        payload: { result: "dispatch_ready" },
      },
    });
    await prisma.outboxEvent.update({
      where: { id: outbox.id },
      data: {
        status: "COMPLETED",
        completedAt: new Date(),
      },
    });

    for (let attempt = 0; attempt < 3; attempt++) {
      const response = await claimAgentJobRoute(
        agentRequest("/api/v1/agent/jobs/claim", {
          deviceId: input.deviceId,
        }),
      );
      const claimed = claimAgentJobResponseSchema.parse(await response.json())
        .data.job;
      expect(claimed).not.toBeNull();
      if (claimed === null) {
        throw new Error("agent_claim_missing");
      }

      await prisma.$executeRaw`
        UPDATE execution_jobs
        SET lease_expires_at = clock_timestamp() - interval '1 second'
        WHERE id = ${claimed.jobId}::uuid
      `;
    }

    const exhaustedResponse = await claimAgentJobRoute(
      agentRequest("/api/v1/agent/jobs/claim", {
        deviceId: input.deviceId,
      }),
    );
    expect(
      claimAgentJobResponseSchema.parse(await exhaustedResponse.json()).data
        .job,
    ).toBeNull();

    const failed = await getRequest.execute(created.request.id);
    expect(failed.status).toBe("failed");
    expect(failed.job.status).toBe("failed");
    expect(failed.job.evidence).toMatchObject([
      { code: "job.simulation.failed" },
    ]);
    await expect(
      prisma.botCase.findUniqueOrThrow({
        where: { requestId: created.request.id },
      }),
    ).resolves.toMatchObject({
      status: "ESCALATED",
      result: "FAILED",
      waitingForUserSince: null,
    });
    const escalatedCase = await prisma.botCase.findUniqueOrThrow({
      where: { requestId: created.request.id },
    });
    expect(escalatedCase.escalatedAt).not.toBeNull();
    const escalation = await prisma.outboxEvent.findFirstOrThrow({
      where: {
        aggregateId: escalatedCase.id,
        eventType: botCaseEscalationRequestedEventType,
      },
    });
    expect(
      botCaseEscalationRequestedEventV1Schema.parse(escalation.payload)
        .reasonCode,
    ).toBe("claim_lease_exhausted");
  });

  async function mutationCounts() {
    const [requests, jobs, cases, tickets, audits, outbox] = await Promise.all([
      prisma.supportRequest.count(),
      prisma.executionJob.count(),
      prisma.botCase.count(),
      prisma.externalTicket.count(),
      prisma.auditEvent.count(),
      prisma.outboxEvent.count(),
    ]);

    return { requests, jobs, cases, tickets, audits, outbox };
  }

  function agentRequest(
    path: string,
    body: unknown,
    idempotencyKey?: string,
  ): Request {
    const headers: Record<string, string> = {
      "content-type": "application/json",
      "x-correlation-id": "agent-integration-correlation",
      "x-development-agent-id": "local-agent-001",
    };
    if (idempotencyKey !== undefined) {
      headers["idempotency-key"] = idempotencyKey;
    }

    return new Request(`http://localhost${path}`, {
      method: "POST",
      headers,
      body: JSON.stringify(body),
    });
  }
});
