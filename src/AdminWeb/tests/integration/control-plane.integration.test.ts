import { beforeAll, afterAll, describe, expect, it } from "vitest";
import {
  catalogResponseSchema,
  createSoftwareInstallationResponseSchema,
  getSupportRequestResponseSchema,
} from "@it-support-native/control-plane-contracts";
import { GET as getCatalog } from "@/app/api/v1/catalog/products/route";
import { GET as getRequestRoute } from "@/app/api/v1/requests/[requestId]/route";
import { POST as createRequestRoute } from "@/app/api/v1/requests/software-installations/route";
import { listCatalogProducts } from "@/modules/catalog/application/list-catalog-products";
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
    await prisma.executionJob.deleteMany();
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

  it("persists request, job, audit, and outbox atomically", async () => {
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
    await expect(prisma.auditEvent.count()).resolves.toBe(1);
    await expect(prisma.outboxEvent.count()).resolves.toBe(1);
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
    await expect(prisma.auditEvent.count()).resolves.toBe(1);
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
        SELECT 'audit_events.created_at', created_at FROM audit_events
        UNION ALL
        SELECT 'outbox_events.created_at', created_at FROM outbox_events
        UNION ALL
        SELECT 'outbox_events.available_at', available_at FROM outbox_events
      ) AS persisted_timestamps
      ORDER BY source
    `;

    expect(timestamps).toHaveLength(9);
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
    await expect(prisma.auditEvent.count()).resolves.toBe(1);
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
    expect(catalogResponse.status).toBe(200);
    expect(
      catalogResponseSchema.parse(await catalogResponse.json()).data.items,
    ).toHaveLength(1);
    await expect(mutationCounts()).resolves.toEqual(countsBefore);
  });

  async function mutationCounts() {
    const [requests, jobs, audits, outbox] = await Promise.all([
      prisma.supportRequest.count(),
      prisma.executionJob.count(),
      prisma.auditEvent.count(),
      prisma.outboxEvent.count(),
    ]);

    return { requests, jobs, audits, outbox };
  }
});
