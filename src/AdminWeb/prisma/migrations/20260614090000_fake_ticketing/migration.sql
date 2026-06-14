-- CreateEnum
CREATE TYPE "TicketingProvider" AS ENUM ('FAKE');

-- CreateEnum
CREATE TYPE "ExternalTicketStatus" AS ENUM ('OPEN');

-- CreateTable
CREATE TABLE "external_tickets" (
    "id" UUID NOT NULL,
    "case_id" UUID NOT NULL,
    "provider" "TicketingProvider" NOT NULL DEFAULT 'FAKE',
    "external_reference" VARCHAR(64) NOT NULL,
    "category" "BotCaseCategory" NOT NULL,
    "status" "ExternalTicketStatus" NOT NULL DEFAULT 'OPEN',
    "correlation_id" VARCHAR(100) NOT NULL,
    "reason_code" VARCHAR(64) NOT NULL,
    "description" VARCHAR(200) NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "external_tickets_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "external_tickets_reason_code_check"
        CHECK ("reason_code" IN ('execution_failed', 'claim_lease_exhausted'))
);

-- CreateIndex
CREATE UNIQUE INDEX "external_tickets_case_id_key"
ON "external_tickets"("case_id");

-- CreateIndex
CREATE UNIQUE INDEX "external_tickets_external_reference_key"
ON "external_tickets"("external_reference");

-- CreateIndex
CREATE INDEX "external_tickets_correlation_id_idx"
ON "external_tickets"("correlation_id");

-- Prevent duplicate escalation publications for one internal case.
CREATE UNIQUE INDEX "outbox_events_one_escalation_per_case_key"
ON "outbox_events"("aggregate_id")
WHERE "event_type" = 'bot-case.escalation-requested.v1';

-- AddForeignKey
ALTER TABLE "external_tickets"
ADD CONSTRAINT "external_tickets_case_id_fkey"
FOREIGN KEY ("case_id") REFERENCES "bot_cases"("id")
ON DELETE RESTRICT ON UPDATE CASCADE;
