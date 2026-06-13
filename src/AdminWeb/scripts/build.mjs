import { spawnSync } from "node:child_process";
import { createRequire } from "node:module";
import { rm } from "node:fs/promises";
import { basename, dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const projectRoot = resolve(dirname(fileURLToPath(import.meta.url)), "..");
const buildDirectory = resolve(projectRoot, ".next");

if (
  dirname(buildDirectory) !== projectRoot ||
  basename(buildDirectory) !== ".next"
) {
  throw new Error(`Unexpected Next.js build directory: ${buildDirectory}`);
}

await rm(buildDirectory, {
  recursive: true,
  force: true,
  maxRetries: 20,
  retryDelay: 250,
});

const require = createRequire(import.meta.url);
const nextCli = require.resolve("next/dist/bin/next");
const result = spawnSync(
  process.execPath,
  [nextCli, "build", "--webpack"],
  {
    cwd: projectRoot,
    stdio: "inherit",
    shell: false,
  },
);

if (result.error !== undefined) {
  throw result.error;
}

if (result.status !== 0) {
  process.exitCode = result.status ?? 1;
}
