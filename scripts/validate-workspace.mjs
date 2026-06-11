import { readFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const repositoryRoot = resolve(dirname(fileURLToPath(import.meta.url)), "..");

const expectedPackages = [
  ["src/AdminWeb/package.json", "@it-support-native/admin-web"],
  ["src/Worker/package.json", "@it-support-native/worker"],
];

for (const [path, expectedName] of expectedPackages) {
  const manifest = JSON.parse(
    await readFile(resolve(repositoryRoot, path), "utf8"),
  );

  if (manifest.name !== expectedName) {
    throw new Error(`${path} must use package name ${expectedName}.`);
  }

  if (manifest.private !== true) {
    throw new Error(`${path} must remain private.`);
  }

  if (Object.hasOwn(manifest, "dependencies")) {
    throw new Error(`${path} must not add runtime dependencies during Block 0.`);
  }
}

console.log("Node workspace boundaries are valid.");
