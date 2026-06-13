import { createHash } from "node:crypto";
import type { CreateConfirmedInstallationCommand } from "../domain/support-request";

export function createRequestPayloadHash(
  command: CreateConfirmedInstallationCommand,
): string {
  const canonicalPayload = JSON.stringify({
    actionId: command.actionId,
    actorSubject: command.actorSubject,
    deviceId: command.deviceId,
    productId: command.productId,
    productVersion: command.productVersion,
  });

  return createHash("sha256").update(canonicalPayload, "utf8").digest("hex");
}
