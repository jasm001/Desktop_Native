import { describe, expect, it } from "vitest";
import {
  isEligibleForNoResponseClosure,
  noResponseClosureHours,
} from "@/modules/cases/domain/bot-case";

const waitingSince = new Date("2026-06-10T12:00:00.000Z");

describe("bot case no-response closure policy", () => {
  it("is not eligible immediately before the 72-hour threshold", () => {
    const now = new Date(
      waitingSince.getTime() + noResponseClosureHours * 60 * 60 * 1_000 - 1,
    );

    expect(
      isEligibleForNoResponseClosure(
        {
          status: "attended_waiting_user",
          waitingForUserSince: waitingSince,
        },
        now,
      ),
    ).toBe(false);
  });

  it("is eligible exactly at and after the 72-hour threshold", () => {
    const threshold = new Date(
      waitingSince.getTime() + noResponseClosureHours * 60 * 60 * 1_000,
    );

    expect(
      isEligibleForNoResponseClosure(
        {
          status: "attended_waiting_user",
          waitingForUserSince: waitingSince,
        },
        threshold,
      ),
    ).toBe(true);
    expect(
      isEligibleForNoResponseClosure(
        {
          status: "attended_waiting_user",
          waitingForUserSince: waitingSince,
        },
        new Date(threshold.getTime() + 1),
      ),
    ).toBe(true);
  });

  it("rejects open, escalated, and incomplete candidates", () => {
    expect(
      isEligibleForNoResponseClosure(
        { status: "open", waitingForUserSince: null },
        new Date("2026-06-20T12:00:00.000Z"),
      ),
    ).toBe(false);
    expect(
      isEligibleForNoResponseClosure(
        { status: "escalated", waitingForUserSince: null },
        new Date("2026-06-20T12:00:00.000Z"),
      ),
    ).toBe(false);
    expect(
      isEligibleForNoResponseClosure(
        {
          status: "attended_waiting_user",
          waitingForUserSince: null,
        },
        new Date("2026-06-20T12:00:00.000Z"),
      ),
    ).toBe(false);
  });
});
