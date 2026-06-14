-- CreateEnum
CREATE TYPE "BotCaseCategory" AS ENUM ('SOFTWARE_INSTALLATION');

-- CreateEnum
CREATE TYPE "BotCaseStatus" AS ENUM (
    'OPEN',
    'ATTENDED_WAITING_USER',
    'ESCALATED'
);

-- CreateEnum
CREATE TYPE "BotCaseResult" AS ENUM ('PENDING', 'SUCCEEDED', 'FAILED');

-- CreateTable
CREATE TABLE "bot_cases" (
    "id" UUID NOT NULL,
    "request_id" UUID NOT NULL,
    "correlation_id" VARCHAR(100) NOT NULL,
    "category" "BotCaseCategory" NOT NULL DEFAULT 'SOFTWARE_INSTALLATION',
    "status" "BotCaseStatus" NOT NULL DEFAULT 'OPEN',
    "result" "BotCaseResult" NOT NULL DEFAULT 'PENDING',
    "waiting_for_user_since" TIMESTAMPTZ(6),
    "escalated_at" TIMESTAMPTZ(6),
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(6) NOT NULL,

    CONSTRAINT "bot_cases_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "bot_cases_state_check"
        CHECK (
            (
                "status" = 'OPEN'
                AND "result" = 'PENDING'
                AND "waiting_for_user_since" IS NULL
                AND "escalated_at" IS NULL
            )
            OR
            (
                "status" = 'ATTENDED_WAITING_USER'
                AND "result" = 'SUCCEEDED'
                AND "waiting_for_user_since" IS NOT NULL
                AND "escalated_at" IS NULL
            )
            OR
            (
                "status" = 'ESCALATED'
                AND "result" = 'FAILED'
                AND "waiting_for_user_since" IS NULL
                AND "escalated_at" IS NOT NULL
            )
        )
);

-- CreateIndex
CREATE UNIQUE INDEX "bot_cases_request_id_key" ON "bot_cases"("request_id");

-- CreateIndex
CREATE INDEX "bot_cases_status_waiting_for_user_since_idx"
ON "bot_cases"("status", "waiting_for_user_since");

-- CreateIndex
CREATE INDEX "bot_cases_correlation_id_idx"
ON "bot_cases"("correlation_id");

-- AddForeignKey
ALTER TABLE "bot_cases"
ADD CONSTRAINT "bot_cases_request_id_fkey"
FOREIGN KEY ("request_id") REFERENCES "support_requests"("id")
ON DELETE RESTRICT ON UPDATE CASCADE;

-- Backfill
INSERT INTO "bot_cases" (
    "id",
    "request_id",
    "correlation_id",
    "category",
    "status",
    "result",
    "waiting_for_user_since",
    "escalated_at",
    "created_at",
    "updated_at"
)
SELECT
    gen_random_uuid(),
    request."id",
    request."correlation_id",
    'SOFTWARE_INSTALLATION'::"BotCaseCategory",
    CASE request."status"
        WHEN 'COMPLETED' THEN 'ATTENDED_WAITING_USER'::"BotCaseStatus"
        WHEN 'FAILED' THEN 'ESCALATED'::"BotCaseStatus"
        ELSE 'OPEN'::"BotCaseStatus"
    END,
    CASE request."status"
        WHEN 'COMPLETED' THEN 'SUCCEEDED'::"BotCaseResult"
        WHEN 'FAILED' THEN 'FAILED'::"BotCaseResult"
        ELSE 'PENDING'::"BotCaseResult"
    END,
    CASE
        WHEN request."status" = 'COMPLETED' THEN request."updated_at"
        ELSE NULL
    END,
    CASE
        WHEN request."status" = 'FAILED' THEN request."updated_at"
        ELSE NULL
    END,
    request."created_at",
    request."updated_at"
FROM "support_requests" AS request;

CREATE TRIGGER "bot_cases_set_timestamps"
BEFORE INSERT OR UPDATE ON "bot_cases"
FOR EACH ROW
EXECUTE FUNCTION set_entity_timestamps();
