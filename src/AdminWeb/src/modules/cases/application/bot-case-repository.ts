import type { BotCase } from "../domain/bot-case";

export interface BotCaseRepository {
  findByRequestId(requestId: string): Promise<BotCase | null>;
}
