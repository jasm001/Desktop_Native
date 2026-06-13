import { rm } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { build } from "esbuild";

const projectRoot = resolve(dirname(fileURLToPath(import.meta.url)), "..");
const outputDirectory = resolve(projectRoot, "dist");

await rm(outputDirectory, { recursive: true, force: true });
await build({
  absWorkingDir: projectRoot,
  bundle: true,
  entryPoints: ["src/main.ts"],
  external: ["pg"],
  format: "esm",
  logLevel: "info",
  outfile: "dist/main.js",
  platform: "node",
  sourcemap: true,
  target: "node24",
});
