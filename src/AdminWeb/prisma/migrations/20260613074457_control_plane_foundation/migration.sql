-- CreateEnum
CREATE TYPE "RequestStatus" AS ENUM ('CONFIRMED', 'COMPLETED', 'FAILED');

-- CreateEnum
CREATE TYPE "JobStatus" AS ENUM ('QUEUED', 'COMPLETED', 'FAILED');

-- CreateEnum
CREATE TYPE "OutboxStatus" AS ENUM ('PENDING', 'PROCESSING', 'COMPLETED', 'FAILED');

-- CreateTable
CREATE TABLE "devices" (
    "id" VARCHAR(128) NOT NULL,
    "display_name" VARCHAR(120) NOT NULL,
    "environment" VARCHAR(32) NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(6) NOT NULL,

    CONSTRAINT "devices_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "support_requests" (
    "id" UUID NOT NULL,
    "reference" VARCHAR(128) NOT NULL,
    "idempotency_key" VARCHAR(100) NOT NULL,
    "idempotency_hash" CHAR(64) NOT NULL,
    "correlation_id" VARCHAR(100) NOT NULL,
    "requester_subject" VARCHAR(128) NOT NULL,
    "device_id" VARCHAR(128) NOT NULL,
    "product_id" VARCHAR(128) NOT NULL,
    "product_version" VARCHAR(128) NOT NULL,
    "action_id" VARCHAR(128) NOT NULL,
    "status" "RequestStatus" NOT NULL DEFAULT 'CONFIRMED',
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(6) NOT NULL,

    CONSTRAINT "support_requests_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "execution_jobs" (
    "id" UUID NOT NULL,
    "request_id" UUID NOT NULL,
    "action_id" VARCHAR(128) NOT NULL,
    "target_id" VARCHAR(128) NOT NULL,
    "target_version" VARCHAR(128) NOT NULL,
    "status" "JobStatus" NOT NULL DEFAULT 'QUEUED',
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMPTZ(6) NOT NULL,

    CONSTRAINT "execution_jobs_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "audit_events" (
    "id" UUID NOT NULL,
    "correlation_id" VARCHAR(100) NOT NULL,
    "actor_subject" VARCHAR(128) NOT NULL,
    "event_type" VARCHAR(128) NOT NULL,
    "entity_type" VARCHAR(64) NOT NULL,
    "entity_id" VARCHAR(128) NOT NULL,
    "payload" JSONB NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "audit_events_pkey" PRIMARY KEY ("id")
);

CREATE OR REPLACE FUNCTION reject_audit_event_mutation()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE EXCEPTION 'audit_events are append-only';
END;
$$;

CREATE TRIGGER "audit_events_append_only"
BEFORE UPDATE OR DELETE ON "audit_events"
FOR EACH ROW
EXECUTE FUNCTION reject_audit_event_mutation();

-- CreateTable
CREATE TABLE "outbox_events" (
    "id" UUID NOT NULL,
    "aggregate_type" VARCHAR(64) NOT NULL,
    "aggregate_id" VARCHAR(128) NOT NULL,
    "event_type" VARCHAR(128) NOT NULL,
    "payload" JSONB NOT NULL,
    "status" "OutboxStatus" NOT NULL DEFAULT 'PENDING',
    "attempt_count" INTEGER NOT NULL DEFAULT 0,
    "available_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "claimed_at" TIMESTAMPTZ(6),
    "claimed_by" VARCHAR(128),
    "completed_at" TIMESTAMPTZ(6),
    "last_error_code" VARCHAR(64),
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "outbox_events_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "outbox_events_attempt_count_check"
        CHECK ("attempt_count" >= 0 AND "attempt_count" <= 3),
    CONSTRAINT "outbox_events_claim_check"
        CHECK (
            ("status" = 'PROCESSING' AND "claimed_at" IS NOT NULL AND "claimed_by" IS NOT NULL)
            OR
            ("status" <> 'PROCESSING' AND "claimed_at" IS NULL AND "claimed_by" IS NULL)
        )
);

-- CreateTable
CREATE TABLE "synthetic_outbox_effects" (
    "id" UUID NOT NULL,
    "outbox_event_id" UUID NOT NULL,
    "effect_type" VARCHAR(128) NOT NULL,
    "payload" JSONB NOT NULL,
    "created_at" TIMESTAMPTZ(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "synthetic_outbox_effects_pkey" PRIMARY KEY ("id")
);

CREATE OR REPLACE FUNCTION set_entity_timestamps()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        NEW."created_at" = clock_timestamp();
    END IF;

    NEW."updated_at" = clock_timestamp();
    RETURN NEW;
END;
$$;

CREATE TRIGGER "devices_set_timestamps"
BEFORE INSERT OR UPDATE ON "devices"
FOR EACH ROW
EXECUTE FUNCTION set_entity_timestamps();

CREATE TRIGGER "support_requests_set_timestamps"
BEFORE INSERT OR UPDATE ON "support_requests"
FOR EACH ROW
EXECUTE FUNCTION set_entity_timestamps();

CREATE TRIGGER "execution_jobs_set_timestamps"
BEFORE INSERT OR UPDATE ON "execution_jobs"
FOR EACH ROW
EXECUTE FUNCTION set_entity_timestamps();

CREATE OR REPLACE FUNCTION set_created_timestamp()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW."created_at" = clock_timestamp();
    RETURN NEW;
END;
$$;

CREATE TRIGGER "audit_events_set_created_at"
BEFORE INSERT ON "audit_events"
FOR EACH ROW
EXECUTE FUNCTION set_created_timestamp();

CREATE TRIGGER "outbox_events_set_created_at"
BEFORE INSERT ON "outbox_events"
FOR EACH ROW
EXECUTE FUNCTION set_created_timestamp();

CREATE TRIGGER "synthetic_outbox_effects_set_created_at"
BEFORE INSERT ON "synthetic_outbox_effects"
FOR EACH ROW
EXECUTE FUNCTION set_created_timestamp();

-- CreateIndex
CREATE UNIQUE INDEX "support_requests_reference_key" ON "support_requests"("reference");

-- CreateIndex
CREATE UNIQUE INDEX "support_requests_idempotency_key_key" ON "support_requests"("idempotency_key");

-- CreateIndex
CREATE INDEX "support_requests_device_id_created_at_idx" ON "support_requests"("device_id", "created_at");

-- CreateIndex
CREATE INDEX "support_requests_correlation_id_idx" ON "support_requests"("correlation_id");

-- CreateIndex
CREATE UNIQUE INDEX "execution_jobs_request_id_key" ON "execution_jobs"("request_id");

-- CreateIndex
CREATE INDEX "execution_jobs_status_created_at_idx" ON "execution_jobs"("status", "created_at");

-- CreateIndex
CREATE INDEX "audit_events_entity_type_entity_id_created_at_idx" ON "audit_events"("entity_type", "entity_id", "created_at");

-- CreateIndex
CREATE INDEX "audit_events_correlation_id_idx" ON "audit_events"("correlation_id");

-- CreateIndex
CREATE INDEX "outbox_events_status_available_at_created_at_idx" ON "outbox_events"("status", "available_at", "created_at");

-- CreateIndex
CREATE INDEX "outbox_events_aggregate_type_aggregate_id_idx" ON "outbox_events"("aggregate_type", "aggregate_id");

-- CreateIndex
CREATE UNIQUE INDEX "synthetic_outbox_effects_outbox_event_id_key" ON "synthetic_outbox_effects"("outbox_event_id");

-- AddForeignKey
ALTER TABLE "support_requests" ADD CONSTRAINT "support_requests_device_id_fkey" FOREIGN KEY ("device_id") REFERENCES "devices"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "execution_jobs" ADD CONSTRAINT "execution_jobs_request_id_fkey" FOREIGN KEY ("request_id") REFERENCES "support_requests"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "synthetic_outbox_effects" ADD CONSTRAINT "synthetic_outbox_effects_outbox_event_id_fkey" FOREIGN KEY ("outbox_event_id") REFERENCES "outbox_events"("id") ON DELETE RESTRICT ON UPDATE CASCADE;
