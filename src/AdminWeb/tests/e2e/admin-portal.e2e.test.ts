import { expect, type Page, test } from "@playwright/test";

const routes = [
  {
    path: "/admin",
    title: "Centro de operaciones",
    active: "Resumen",
    landmark: "Servicios disponibles",
  },
  {
    path: "/admin/catalog",
    title: /Cat.+logo controlado/u,
    active: /Cat.+logo/u,
    landmark: "Productos versionados",
  },
  {
    path: "/admin/operations",
    title: "Actividad reciente",
    active: "Operaciones",
    landmark: "Solicitudes recientes",
  },
  {
    path: "/admin/audit",
    title: "Registro de evidencia",
    active: /Auditor/u,
    landmark: "Eventos recientes",
  },
  {
    path: "/admin/access",
    title: "Identidad y acceso",
    active: "Acceso",
    landmark: "Superficie protegida",
  },
  {
    path: "/admin/approvals",
    title: "Aprobaciones pendientes",
    active: "Aprobaciones",
    landmark: "Superficie protegida",
  },
  {
    path: "/admin/support",
    title: "Tickets y soporte remoto",
    active: "Soporte",
    landmark: "Superficie protegida",
  },
  {
    path: "/admin/reporting",
    title: "Reportes y configuracion",
    active: "Reportes",
    landmark: "Superficie protegida",
  },
] as const;

for (const route of routes) {
  test(`renders ${route.path} as a protected read-only view`, async ({ page }) => {
    const consoleMessages = collectConsoleMessages(page);

    await page.goto(route.path);

    await expect(page).toHaveTitle(/IT Support Native/u);
    await expect(
      page.getByRole("heading", { name: route.title, level: 1 }),
    ).toBeVisible();
    await expect(
      page.getByRole("heading", { name: route.landmark }),
    ).toBeVisible();
    await expect(
      page.locator(".read-only-state").getByText("Solo lectura"),
    ).toBeVisible();
    await expect(
      page.getByLabel("Identidad actual del portal"),
    ).toContainText("DeveloperAllAccess");
    await expect(
      page.getByRole("navigation").getByRole("link", { name: route.active }),
    ).toHaveAttribute("aria-current", "page");
    await expect(page.locator("form")).toHaveCount(0);
    await expect(page.locator("main").getByRole("button")).toHaveCount(0);
    await expect(page.getByText(/Unhandled Runtime Error|Application error/u)).toHaveCount(0);

    await expectNoPageOverflow(page);
    expect(consoleMessages).toEqual([]);
  });
}

test("supports keyboard navigation and skip-link focus", async ({ page }) => {
  const consoleMessages = collectConsoleMessages(page);

  await page.goto("/admin");

  await page.keyboard.press("Tab");
  await expect(
    page.getByRole("link", { name: "Saltar al contenido principal" }),
  ).toBeFocused();

  await page.getByRole("navigation").getByRole("link", { name: /Cat.+logo/u }).focus();
  await page.keyboard.press("Enter");
  await expect(page).toHaveURL(/\/admin\/catalog$/u);
  await expect(
    page.getByRole("heading", { name: /Cat.+logo controlado/u, level: 1 }),
  ).toBeVisible();
  expect(consoleMessages).toEqual([]);
});

function collectConsoleMessages(page: Page): string[] {
  const messages: string[] = [];
  page.on("console", (message) => {
    if (message.type() === "error" || message.type() === "warning") {
      messages.push(message.text());
    }
  });

  return messages;
}

async function expectNoPageOverflow(page: Page) {
  const metrics = await page.evaluate(() => {
    const tolerance = 1;
    const width = window.innerWidth;
    const overflowing = Array.from(document.body.querySelectorAll("*"))
      .filter((element) => {
        if (element.closest(".admin-table-wrapper")) {
          return false;
        }

        const rect = element.getBoundingClientRect();
        return rect.left < -tolerance || rect.right > width + tolerance;
      })
      .map((element) => {
        const rect = element.getBoundingClientRect();
        return {
          tag: element.tagName,
          className: String(element.getAttribute("class") ?? ""),
          left: rect.left,
          right: rect.right,
        };
      });

    return {
      innerWidth: width,
      documentWidth: document.documentElement.scrollWidth,
      bodyWidth: document.body.scrollWidth,
      overflowing,
    };
  });

  expect(metrics.documentWidth).toBeLessThanOrEqual(metrics.innerWidth);
  expect(metrics.bodyWidth).toBeLessThanOrEqual(metrics.innerWidth);
  expect(metrics.overflowing).toEqual([]);
}
