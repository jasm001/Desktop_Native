import type { PrismaClient } from "../../../generated/prisma/client";
import type { BotCaseRepository } from "../application/bot-case-repository";
import type { BotCase } from "../domain/bot-case";
import { mapBotCase } from "./bot-case-mapper";

export class PrismaBotCaseRepository implements BotCaseRepository {
  public constructor(private readonly prisma: PrismaClient) {}

  public async findByRequestId(requestId: string): Promise<BotCase | null> {
    const botCase = await this.prisma.botCase.findUnique({
      where: { requestId },
    });

    return botCase === null ? null : mapBotCase(botCase);
  }
}
