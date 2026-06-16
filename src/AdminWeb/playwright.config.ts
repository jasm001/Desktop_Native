import { defineConfig, devices } from "@playwright/test";

const baseURL = process.env.IT_SUPPORT_ADMIN_E2E_BASE_URL;
const chromiumChannel = process.env.PLAYWRIGHT_CHROMIUM_CHANNEL ?? "msedge";

export default defineConfig({
  testDir: "./tests/e2e",
  reporter: "list",
  timeout: 30_000,
  expect: {
    timeout: 5_000,
  },
  use: {
    baseURL,
    colorScheme: "light",
    screenshot: "only-on-failure",
    trace: "retain-on-failure",
  },
  projects: [
    {
      name: "desktop",
      use: {
        ...devices["Desktop Chrome"],
        channel: chromiumChannel,
        viewport: { width: 1440, height: 1000 },
      },
    },
    {
      name: "mobile",
      use: {
        ...devices["Pixel 5"],
        channel: chromiumChannel,
        viewport: { width: 390, height: 844 },
      },
    },
  ],
});
