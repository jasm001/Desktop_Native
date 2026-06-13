-- AlterEnum
ALTER TYPE "JobStatus" ADD VALUE IF NOT EXISTS 'RUNNING';

-- AlterTable
ALTER TABLE "execution_jobs"
ADD COLUMN "attempt_count" INTEGER NOT NULL DEFAULT 0,
ADD COLUMN "claimed_at" TIMESTAMPTZ(6),
ADD COLUMN "claimed_by" VARCHAR(128),
ADD COLUMN "claim_token_hash" CHAR(64),
ADD COLUMN "lease_expires_at" TIMESTAMPTZ(6),
ADD COLUMN "result_idempotency_key" VARCHAR(100),
ADD COLUMN "result_payload_hash" CHAR(64),
ADD COLUMN "completed_at" TIMESTAMPTZ(6),
ADD CONSTRAINT "execution_jobs_attempt_count_check"
    CHECK ("attempt_count" >= 0 AND "attempt_count" <= 3),
ADD CONSTRAINT "execution_jobs_claim_check"
    CHECK (
        (
            "status" = 'RUNNING'
            AND "claimed_at" IS NOT NULL
            AND "claimed_by" IS NOT NULL
            AND "claim_token_hash" IS NOT NULL
            AND "lease_expires_at" IS NOT NULL
        )
        OR
        (
            "status" <> 'RUNNING'
            AND "claimed_at" IS NULL
            AND "claimed_by" IS NULL
            AND "claim_token_hash" IS NULL
            AND "lease_expires_at" IS NULL
        )
    );

-- CreateTable
CREATE TABLE "execution_evidence" (
    "id" UUID NOT NULL,
    "job_id" UUID NOT NULL,
    "sequence" INTEGER NOT NULL,
    "code" VARCHAR(128) NOT NULL,
    "summary" VARCHAR(200) NOT NULL,
    "recorded_at" TIMESTAMPTZ(6) NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "execution_evidence_pkey" PRIMARY KEY ("id")
);

CREATE TRIGGER "execution_evidence_set_created_at"
BEFORE INSERT ON "execution_evidence"
FOR EACH ROW
EXECUTE FUNCTION set_created_timestamp();

CREATE UNIQUE INDEX "execution_jobs_result_idempotency_key_key"
ON "execution_jobs"("result_idempotency_key");

CREATE INDEX "execution_jobs_status_lease_expires_at_created_at_idx"
ON "execution_jobs"("status", "lease_expires_at", "created_at");

CREATE UNIQUE INDEX "execution_evidence_job_id_sequence_key"
ON "execution_evidence"("job_id", "sequence");

CREATE INDEX "execution_evidence_job_id_recorded_at_idx"
ON "execution_evidence"("job_id", "recorded_at");

ALTER TABLE "execution_evidence"
ADD CONSTRAINT "execution_evidence_job_id_fkey"
FOREIGN KEY ("job_id") REFERENCES "execution_jobs"("id")
ON DELETE RESTRICT ON UPDATE CASCADE;
