import type { BotCaseView } from "@it-support-native/control-plane-contracts";
import type { BotCase as PrismaBotCase } from "../../../generated/prisma/client";

export function mapBotCase(botCase: PrismaBotCase): BotCaseView {
  return {
    id: botCase.id,
    requestId: botCase.requestId,
    correlationId: botCase.correlationId,
    category: botCase.category.toLowerCase() as BotCaseView["category"],
    status: botCase.status.toLowerCase() as BotCaseView["status"],
    result: botCase.result.toLowerCase() as BotCaseView["result"],
    waitingForUserSince: botCase.waitingForUserSince?.toISOString() ?? null,
    escalatedAt: botCase.escalatedAt?.toISOString() ?? null,
    createdAt: botCase.createdAt.toISOString(),
    updatedAt: botCase.updatedAt.toISOString(),
  };
}
