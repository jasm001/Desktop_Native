import { expect, type Page, test } from "@playwright/test";

const protectedRoutes = ["/admin", "/admin/access", "/admin/support"] as const;

for (const route of protectedRoutes) {
  test(`fails closed on ${route} without rendering protected portal content`, async ({
    page,
  }) => {
    const consoleMessages = collectConsoleMessages(page);

    await page.goto(route);

    await expect(
      page.getByRole("heading", {
        name: "Acceso administrativo no disponible",
        level: 1,
      }),
    ).toBeVisible();
    await expect(
      page.getByRole("link", { name: "Volver al estado del plano de control" }),
    ).toHaveAttribute("href", "/");
    await expect(page.getByText("Centro de operaciones")).toHaveCount(0);
    await expect(page.getByText("Identidad y acceso")).toHaveCount(0);
    await expect(page.getByText("Tickets y soporte remoto")).toHaveCount(0);
    await expect(page.getByText("Operador de desarrollo")).toHaveCount(0);
    await expect(page.locator("form")).toHaveCount(0);
    await expect(page.locator("main").getByRole("button")).toHaveCount(0);
    await expect(page.getByText(/Unhandled Runtime Error|Application error/u)).toHaveCount(0);
    expect(consoleMessages).toEqual([]);
  });
}

function collectConsoleMessages(page: Page): string[] {
  const messages: string[] = [];
  page.on("console", (message) => {
    if (message.type() === "error" || message.type() === "warning") {
      messages.push(message.text());
    }
  });

  return messages;
}
