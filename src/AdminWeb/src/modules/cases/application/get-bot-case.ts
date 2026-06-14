import { ApplicationError } from "../../../platform/errors/application-error";
import type { BotCase } from "../domain/bot-case";
import type { BotCaseRepository } from "./bot-case-repository";

export class GetBotCase {
  public constructor(private readonly repository: BotCaseRepository) {}

  public async execute(requestId: string): Promise<BotCase> {
    const botCase = await this.repository.findByRequestId(requestId);

    if (botCase === null) {
      throw new ApplicationError(
        "case_not_found",
        404,
        "The bot case was not found.",
      );
    }

    return botCase;
  }
}
