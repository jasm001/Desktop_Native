import { setTimeout as delay } from "node:timers/promises";
import { closeWorkerPool, getWorkerPool } from "./platform/database";
import { processNextOutboxEvent } from "./outbox/process-next-event";

const workerId = `worker-${crypto.randomUUID()}`;
const runOnce = process.argv.includes("--once");
const pool = getWorkerPool();
const shutdown = new AbortController();

process.once("SIGINT", () => {
  shutdown.abort();
});
process.once("SIGTERM", () => {
  shutdown.abort();
});

try {
  for (;;) {
    const result = await processNextOutboxEvent(pool, workerId);
    if (runOnce || shutdown.signal.aborted) {
      break;
    }

    if (result.kind === "idle") {
      await delay(1_000, undefined, { signal: shutdown.signal });
    }
  }
} catch (error) {
  if (!shutdown.signal.aborted) {
    throw error;
  }
} finally {
  await closeWorkerPool();
}
