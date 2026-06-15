import { describe, expect, it } from "vitest";
import type {
  AdminAuditSummary,
  AdminOperationSummary,
  AdminReadRepository,
} from "@/modules/administration/application/admin-read-repository";
import {
  adminReadPageLimit,
  getRecentAdminAuditEvents,
  getRecentAdminOperations,
} from "@/modules/administration/application/get-admin-read-model";

class RecordingAdminReadRepository implements AdminReadRepository {
  public operationsLimit: number | null = null;
  public auditLimit: number | null = null;

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
});
