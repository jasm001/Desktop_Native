import { randomUUID } from "node:crypto";
import { spawn, spawnSync } from "node:child_process";
import { createServer } from "node:net";
import { resolve } from "node:path";
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

let webProcess;
let workerProcess;

try {
  await administrationClient.query(`CREATE DATABASE "${databaseName}"`);

  const childEnvironment = {
    ...process.env,
    DATABASE_URL: testConnectionString,
    IT_SUPPORT_ENVIRONMENT: "development",
    LOCAL_DEVELOPMENT_IDENTITY_ENABLED: "true",
    LOCAL_DEVELOPMENT_AGENT_ENABLED: "true",
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

  if (process.platform === "win32") {
    await runWindowsEndToEnd(testConnectionString, childEnvironment);
  } else {
    console.log(
      "Skipping the Windows-only WinUI and DeviceAgent end-to-end flow.",
    );
  }
} finally {
  await stopChild(workerProcess);
  await stopChild(webProcess);
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

function runDotnet(argumentsList, environment) {
  const result = spawnSync("dotnet", argumentsList, {
    cwd: process.cwd(),
    env: environment,
    stdio: "inherit",
    shell: false,
  });

  if (result.error !== undefined) {
    throw result.error;
  }

  if (result.status !== 0) {
    throw new Error(
      `dotnet command failed with exit code ${String(result.status)}.`,
    );
  }
}

async function runWindowsEndToEnd(connectionString, childEnvironment) {
  await resetBusinessData(connectionString);
  if (process.env.IT_SUPPORT_CONTROL_PLANE_PREBUILT !== "true") {
    runPnpm(
      ["--filter", "@it-support-native/admin-web", "run", "build"],
      childEnvironment,
    );
    runPnpm(
      ["--filter", "@it-support-native/worker", "run", "build"],
      childEnvironment,
    );
  }

  const port = await getAvailablePort();
  const controlPlaneUrl = `http://127.0.0.1:${String(port)}/`;
  const runtimeEnvironment = {
    ...childEnvironment,
    IT_SUPPORT_CONTROL_PLANE_URL: controlPlaneUrl,
    LOCAL_CONTROL_PLANE_ENABLED: "true",
    NODE_ENV: "production",
  };
  const standaloneRoot = resolve(
    "src",
    "AdminWeb",
    ".next",
    "standalone",
    "src",
    "AdminWeb",
  );
  webProcess = spawn(
    process.execPath,
    [resolve(standaloneRoot, "server.js")],
    {
      cwd: standaloneRoot,
      env: {
        ...runtimeEnvironment,
        HOSTNAME: "127.0.0.1",
        PORT: String(port),
      },
      stdio: "inherit",
      windowsHide: true,
    },
  );
  workerProcess = spawn(
    process.execPath,
    [resolve("src", "Worker", "dist", "main.js")],
    {
      cwd: process.cwd(),
      env: runtimeEnvironment,
      stdio: "inherit",
      windowsHide: true,
    },
  );

  await waitForControlPlane(controlPlaneUrl);
  runDotnet(
    [
      "restore",
      "tests/EndToEnd/ITSupportNative.EndToEnd.csproj",
      "--locked-mode",
      "-m:1",
      "--disable-build-servers",
    ],
    runtimeEnvironment,
  );
  runDotnet(
    [
      "run",
      "--project",
      "tests/EndToEnd/ITSupportNative.EndToEnd.csproj",
      "--configuration",
      "Release",
      "--no-restore",
      "--disable-build-servers",
    ],
    runtimeEnvironment,
  );
  await verifyEndToEndState(connectionString);
}

async function resetBusinessData(connectionString) {
  const client = new Client({ connectionString });
  await client.connect();
  try {
    await client.query("DELETE FROM synthetic_outbox_effects");
    await client.query("DELETE FROM outbox_events");
    await client.query("DELETE FROM execution_evidence");
    await client.query("DELETE FROM execution_jobs");
    await client.query("DELETE FROM support_requests");
  } finally {
    await client.end();
  }
}

async function verifyEndToEndState(connectionString) {
  const client = new Client({ connectionString });
  await client.connect();
  try {
    const deadline = Date.now() + 10_000;
    for (;;) {
      const result = await client.query(
        `
          SELECT
            (SELECT count(*) FROM support_requests) AS requests,
            (SELECT count(*) FROM execution_jobs WHERE status = 'COMPLETED') AS completed_jobs,
            (SELECT count(*) FROM execution_evidence) AS evidence,
            (SELECT count(*) FROM outbox_events WHERE status = 'COMPLETED') AS completed_outbox,
            (SELECT count(*) FROM synthetic_outbox_effects) AS effects
        `,
      );
      const row = result.rows[0];
      if (
        row?.requests === "1" &&
        row.completed_jobs === "1" &&
        row.evidence === "3" &&
        row.completed_outbox === "2" &&
        row.effects === "2"
      ) {
        return;
      }

      if (Date.now() >= deadline) {
        throw new Error(
          `The end-to-end database state was incomplete: ${JSON.stringify(row)}`,
        );
      }

      await new Promise((resolveDelay) => setTimeout(resolveDelay, 250));
    }
  } finally {
    await client.end();
  }
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

async function waitForControlPlane(baseUrl) {
  const deadline = Date.now() + 30_000;
  for (;;) {
    try {
      const response = await fetch(`${baseUrl}api/v1/catalog/products?limit=1`);
      if (response.ok) {
        return;
      }
    } catch {
      // The server may still be starting.
    }

    if (Date.now() >= deadline) {
      throw new Error("The local control plane did not become ready.");
    }

    await new Promise((resolveDelay) => setTimeout(resolveDelay, 250));
  }
}

async function stopChild(child) {
  if (child === undefined || child.exitCode !== null) {
    return;
  }

  child.kill();
  await Promise.race([
    new Promise((resolveExit) => child.once("exit", resolveExit)),
    new Promise((resolveDelay) => setTimeout(resolveDelay, 5_000)),
  ]);
}
