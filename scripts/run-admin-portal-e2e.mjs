import { randomUUID } from "node:crypto";
import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import { createServer } from "node:net";
import { resolve } from "node:path";
import process from "node:process";
import { fileURLToPath } from "node:url";
import pg from "pg";

const { Client } = pg;
const packageName = "@it-support-native/admin-web";
const baseConnectionString = process.env.DATABASE_URL;
const repositoryRoot = fileURLToPath(new URL("..", import.meta.url));
const adminWebRoot = resolve(repositoryRoot, "src", "AdminWeb");
const nodeExecutable = existsSync(process.execPath) ? process.execPath : "node";

if (
  baseConnectionString === undefined ||
  baseConnectionString.trim().length === 0
) {
  throw new Error("DATABASE_URL is required for admin portal e2e tests.");
}

const databaseName = `it_support_native_admin_e2e_${randomUUID().replaceAll("-", "")}`;
const databaseUrl = new URL(baseConnectionString);
databaseUrl.pathname = `/${databaseName}`;
const testConnectionString = databaseUrl.toString();

const administrationClient = new Client({
  connectionString: baseConnectionString,
  connectionTimeoutMillis: 5_000,
  application_name: "it-support-native-admin-e2e-harness",
});

await administrationClient.connect();

let nextProcess;

try {
  await administrationClient.query(`CREATE DATABASE "${databaseName}"`);

  const sharedEnvironment = {
    ...process.env,
    DATABASE_URL: testConnectionString,
    IT_SUPPORT_ENVIRONMENT: "development",
    NEXT_TELEMETRY_DISABLED: "1",
  };

  runPnpm(
    ["--filter", packageName, "run", "prisma:migrate:deploy"],
    sharedEnvironment,
  );
  runPnpm(
    ["--filter", packageName, "run", "db:seed:development"],
    {
      ...sharedEnvironment,
      LOCAL_DEVELOPMENT_IDENTITY_ENABLED: "true",
    },
  );

  await runPortalTests(
    {
      ...sharedEnvironment,
      LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED: "false",
      LOCAL_DEVELOPMENT_PORTAL_ROLE: "",
    },
    "tests/e2e/admin-access-denied.e2e.test.ts",
  );

  await runPortalTests(
    {
      ...sharedEnvironment,
      LOCAL_DEVELOPMENT_PORTAL_IDENTITY_ENABLED: "true",
      LOCAL_DEVELOPMENT_PORTAL_ROLE: "DeveloperAllAccess",
    },
    "tests/e2e/admin-portal.e2e.test.ts",
  );
} finally {
  await stopNext();
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

async function runPortalTests(environment, testFile) {
  await stopNext();
  const port = await getAvailablePort();
  const baseUrl = `http://127.0.0.1:${String(port)}`;

  nextProcess = spawnNextDev(
    [
      "dev",
      "--hostname",
      "127.0.0.1",
      "--port",
      String(port),
    ],
    {
      ...environment,
      HOSTNAME: "127.0.0.1",
      PORT: String(port),
    },
  );

  await waitForAdminPortal(baseUrl);

  runPnpm(
    [
      "--filter",
      packageName,
      "exec",
      "playwright",
      "test",
      "--config",
      "playwright.config.ts",
      testFile,
    ],
    {
      ...environment,
      IT_SUPPORT_ADMIN_E2E_BASE_URL: baseUrl,
    },
  );
}

function runPnpm(argumentsList, environment) {
  const command = pnpmCommand(argumentsList);
  const result = spawnSync(command.executable, command.argumentsList, {
    cwd: repositoryRoot,
    env: environment,
    stdio: "inherit",
    shell: false,
  });

  if (result.error !== undefined) {
    throw result.error;
  }

  if (result.status !== 0) {
    throw new Error(
      `pnpm command failed with exit code ${String(result.status)}.`,
    );
  }
}

function spawnPnpm(argumentsList, environment) {
  const command = pnpmCommand(argumentsList);
  return spawn(command.executable, command.argumentsList, {
    cwd: repositoryRoot,
    env: environment,
    stdio: "inherit",
    shell: false,
    windowsHide: true,
  });
}

function spawnNextDev(argumentsList, environment) {
  const nextBin = resolve(
    adminWebRoot,
    "node_modules",
    "next",
    "dist",
    "bin",
    "next",
  );

  return spawn(nodeExecutable, [nextBin, ...argumentsList], {
    cwd: adminWebRoot,
    env: environment,
    stdio: "inherit",
    shell: false,
    windowsHide: true,
  });
}

function pnpmCommand(argumentsList) {
  const pnpmEntryPoint = process.env.npm_execpath;
  if (pnpmEntryPoint !== undefined) {
    return {
      executable: nodeExecutable,
      argumentsList: [pnpmEntryPoint, ...argumentsList],
    };
  }

  return {
    executable: process.platform === "win32" ? "corepack.cmd" : "corepack",
    argumentsList: ["pnpm@11.5.3", ...argumentsList],
  };
}

async function waitForAdminPortal(baseUrl) {
  const deadline = Date.now() + 45_000;

  for (;;) {
    try {
      const response = await fetch(`${baseUrl}/admin`);
      if (response.ok) {
        return;
      }
    } catch {
      // The Next.js development server may still be starting.
    }

    if (Date.now() >= deadline) {
      throw new Error("The admin portal did not become ready.");
    }

    await new Promise((resolveDelay) => setTimeout(resolveDelay, 250));
  }
}

async function stopNext() {
  if (nextProcess === undefined || nextProcess.exitCode !== null) {
    nextProcess = undefined;
    return;
  }

  nextProcess.kill();
  await Promise.race([
    new Promise((resolveExit) => nextProcess.once("exit", resolveExit)),
    new Promise((resolveDelay) => setTimeout(resolveDelay, 5_000)),
  ]);
  nextProcess = undefined;
}

async function getAvailablePort() {
  return await new Promise((resolvePort, rejectPort) => {
    const server = createServer();
    server.unref();
    server.on("error", rejectPort);
    server.listen(0, "127.0.0.1", () => {
      const address = server.address();
      if (address === null || typeof address === "string") {
        server.close();
        rejectPort(new Error("Unable to allocate a loopback port."));
        return;
      }

      server.close(() => resolvePort(address.port));
    });
  });
}
