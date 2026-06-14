import type { BotCaseView } from "@it-support-native/control-plane-contracts";

export type BotCase = BotCaseView;

export interface NoResponseClosureCandidate {
  readonly status: BotCase["status"];
  readonly waitingForUserSince: Date | null;
}

export const noResponseClosureHours = 72;

export function isEligibleForNoResponseClosure(
  candidate: NoResponseClosureCandidate,
  now: Date,
): boolean {
  if (
    candidate.status !== "attended_waiting_user" ||
    candidate.waitingForUserSince === null
  ) {
    return false;
  }

  const threshold =
    candidate.waitingForUserSince.getTime() +
    noResponseClosureHours * 60 * 60 * 1_000;

  return now.getTime() >= threshold;
}
