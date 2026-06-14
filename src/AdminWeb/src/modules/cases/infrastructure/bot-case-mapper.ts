import type { BotCaseView } from "@it-support-native/control-plane-contracts";
import type { Prisma } from "../../../generated/prisma/client";

type BotCaseWithTicket = Prisma.BotCaseGetPayload<{
  include: { externalTicket: true };
}>;

export function mapBotCase(botCase: BotCaseWithTicket): BotCaseView {
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
    externalTicket:
      botCase.externalTicket === null
        ? null
        : {
            id: botCase.externalTicket.id,
            provider: "fake",
            reference: botCase.externalTicket.externalReference,
            category:
              botCase.externalTicket.category.toLowerCase() as BotCaseView["category"],
            status: "open",
            correlationId: botCase.externalTicket.correlationId,
            reasonCode:
              botCase.externalTicket.reasonCode as NonNullable<
                BotCaseView["externalTicket"]
              >["reasonCode"],
            description: botCase.externalTicket.description,
            createdAt: botCase.externalTicket.createdAt.toISOString(),
          },
  };
}
