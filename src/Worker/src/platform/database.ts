import { Pool } from "pg";

let pool: Pool | undefined;

export function getWorkerPool(): Pool {
  if (pool !== undefined) {
    return pool;
  }

  const connectionString = process.env.DATABASE_URL;
  if (connectionString === undefined || connectionString.trim().length === 0) {
    throw new Error("database_configuration_unavailable");
  }

  pool = new Pool({
    connectionString,
    connectionTimeoutMillis: 5_000,
    idleTimeoutMillis: 10_000,
    max: 5,
    application_name: "it-support-native-worker",
  });

  return pool;
}

export async function closeWorkerPool(): Promise<void> {
  if (pool !== undefined) {
    await pool.end();
    pool = undefined;
  }
}
