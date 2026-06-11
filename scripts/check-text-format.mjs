import { readFile, readdir, stat } from "node:fs/promises";
import { extname, join } from "node:path";

const ignoredDirectories = new Set([
  ".dotnet",
  ".dotnet-home",
  ".git",
  ".packages",
  ".tools",
  ".vs",
  "bin",
  "node_modules",
  "obj",
  "reference",
]);

const checkedExtensions = new Set([
  ".cs",
  ".csproj",
  ".editorconfig",
  ".gitattributes",
  ".gitignore",
  ".json",
  ".md",
  ".mjs",
  ".props",
  ".ps1",
  ".slnx",
  ".targets",
  ".yaml",
  ".yml",
]);

const failures = [];

async function visit(path) {
  for (const entry of await readdir(path)) {
    if (ignoredDirectories.has(entry) || entry === "packages.lock.json") {
      continue;
    }

    const fullPath = join(path, entry);
    const info = await stat(fullPath);

    if (info.isDirectory()) {
      await visit(fullPath);
      continue;
    }

    if (!checkedExtensions.has(extname(entry)) && !checkedExtensions.has(entry)) {
      continue;
    }

    const content = await readFile(fullPath, "utf8");
    if (content.length > 0 && !content.endsWith("\n")) {
      failures.push(`${fullPath}: missing final newline`);
    }

    content.split(/\r?\n/u).forEach((line, index) => {
      if (/[ \t]+$/u.test(line)) {
        failures.push(`${fullPath}:${index + 1}: trailing whitespace`);
      }
    });
  }
}

await visit(".");

if (failures.length > 0) {
  console.error(failures.join("\n"));
  process.exitCode = 1;
} else {
  console.log("Text format checks passed.");
}
