import type { SupportRequestView } from "@it-support-native/control-plane-contracts";
import type { Prisma } from "../../../generated/prisma/client";

export type RequestWithJob = Prisma.SupportRequestGetPayload<{
  include: { job: { include: { evidence: true } } };
}>;

export function mapSupportRequest(
  request: RequestWithJob,
): SupportRequestView {
  if (request.job === null) {
    throw new Error("support_request_job_missing");
  }

  return {
    id: request.id,
    reference: request.reference,
    correlationId: request.correlationId,
    status: request.status.toLowerCase() as SupportRequestView["status"],
    deviceId: request.deviceId,
    productId: request.productId,
    productVersion: request.productVersion,
    actionId: request.actionId,
    createdAt: request.createdAt.toISOString(),
    job: {
      id: request.job.id,
      status: request.job.status.toLowerCase() as SupportRequestView["job"]["status"],
      evidence: request.job.evidence
        .toSorted((left, right) => left.sequence - right.sequence)
        .map((item) => ({
          code: item.code as SupportRequestView["job"]["evidence"][number]["code"],
          summary: item.summary,
          recordedAt: item.recordedAt.toISOString(),
        })),
    },
  };
}
