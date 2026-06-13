import { randomUUID } from "node:crypto";
import { spawnSync } from "node:child_process";
import process from "node:process";
import pg from "pg";

const { Client } = pg;
const baseConnectionString = process.env.DATABASE_URL;

if (
  baseConnectionString === undefined ||
  baseConnectionString.trim().length === 0
) {
  throw new Error(
    "DATABASE_URL is required for PostgreSQL integration tests.",
  );
}

const databaseName = `it_support_native_test_${randomUUID().replaceAll("-", "")}`;
const databaseUrl = new URL(baseConnectionString);
databaseUrl.pathname = `/${databaseName}`;
const testConnectionString = databaseUrl.toString();

const administrationClient = new Client({
  connectionString: baseConnectionString,
  connectionTimeoutMillis: 5_000,
  application_name: "it-support-native-integration-harness",
});

await administrationClient.connect();

try {
  await administrationClient.query(`CREATE DATABASE "${databaseName}"`);

  const childEnvironment = {
    ...process.env,
    DATABASE_URL: testConnectionString,
    IT_SUPPORT_ENVIRONMENT: "development",
    LOCAL_DEVELOPMENT_IDENTITY_ENABLED: "true",
  };

  runPnpm(
    ["--filter", "@it-support-native/admin-web", "run", "prisma:migrate:deploy"],
    childEnvironment,
  );
  runPnpm(
    [
      "--filter",
      "@it-support-native/admin-web",
      "run",
      "db:seed:development",
    ],
    childEnvironment,
  );
  runPnpm(
    [
      "--filter",
      "@it-support-native/admin-web",
      "run",
      "test:integration:direct",
    ],
    childEnvironment,
  );
  runPnpm(
    [
      "--filter",
      "@it-support-native/worker",
      "run",
      "test:integration:direct",
    ],
    childEnvironment,
  );
} finally {
  await administrationClient.query(
    `
      SELECT pg_terminate_backend(pid)
      FROM pg_stat_activity
      WHERE datname = $1
        AND pid <> pg_backend_pid()
    `,
    [databaseName],
  );
  await administrationClient.query(`DROP DATABASE IF EXISTS "${databaseName}"`);
  await administrationClient.end();
}

function runPnpm(argumentsList, environment) {
  const pnpmEntryPoint = process.env.npm_execpath;
  if (pnpmEntryPoint === undefined) {
    throw new Error("The integration harness must run through pnpm.");
  }

  const result = spawnSync(
    process.execPath,
    [pnpmEntryPoint, ...argumentsList],
    {
      cwd: process.cwd(),
      env: environment,
      stdio: "inherit",
      shell: false,
    },
  );

  if (result.error !== undefined) {
    throw result.error;
  }

  if (result.status !== 0) {
    throw new Error(
      `pnpm command failed with exit code ${String(result.status)}.`,
    );
  }
}
