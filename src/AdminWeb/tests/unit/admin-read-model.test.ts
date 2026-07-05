import { describe, expect, it } from "vitest";
import type {
  AdminAuditSummary,
  AdminLabReadModel,
  AdminOperationSummary,
  AdminReadRepository,
} from "@/modules/administration/application/admin-read-repository";
import { getAdminLabReadModel } from "@/modules/administration/application/get-admin-lab-read-model";
import {
  adminReadPageLimit,
  getRecentAdminAuditEvents,
  getRecentAdminOperations,
} from "@/modules/administration/application/get-admin-read-model";

class RecordingAdminReadRepository implements AdminReadRepository {
  public operationsLimit: number | null = null;
  public auditLimit: number | null = null;
  public labLimit: number | null = null;

  public listRecentOperations(
    limit: number,
  ): Promise<readonly AdminOperationSummary[]> {
    this.operationsLimit = limit;
    return Promise.resolve([]);
  }

  public listRecentAuditEvents(
    limit: number,
  ): Promise<readonly AdminAuditSummary[]> {
    this.auditLimit = limit;
    return Promise.resolve([]);
  }

  public getLabReadModel(limit: number): Promise<AdminLabReadModel> {
    this.labLimit = limit;
    return Promise.resolve({
      components: [],
      metrics: [],
      recentOperations: [],
      recentAuditEvents: [],
      recentOutboxEvents: [],
      recentExternalTickets: [],
      boundaries: [],
    });
  }
}

describe("admin read model", () => {
  it("applies the fixed bounded limit to operational reads", async () => {
    const repository = new RecordingAdminReadRepository();

    await expect(getRecentAdminOperations(repository)).resolves.toEqual([]);
    expect(repository.operationsLimit).toBe(adminReadPageLimit);
  });

  it("applies the fixed bounded limit to audit reads", async () => {
    const repository = new RecordingAdminReadRepository();

    await expect(getRecentAdminAuditEvents(repository)).resolves.toEqual([]);
    expect(repository.auditLimit).toBe(adminReadPageLimit);
  });

  it("applies the fixed bounded limit to lab-real reads", async () => {
    const repository = new RecordingAdminReadRepository();

    await expect(getAdminLabReadModel(repository)).resolves.toMatchObject({
      components: [],
      metrics: [],
    });
    expect(repository.labLimit).toBe(adminReadPageLimit);
  });
});
