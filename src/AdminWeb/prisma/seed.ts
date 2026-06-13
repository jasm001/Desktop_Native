import { getDevelopmentIdentity } from "../src/modules/identity/application/development-identity";
import { getPrisma } from "../src/platform/db/prisma";

getDevelopmentIdentity();

const prisma = getPrisma();

await prisma.device.upsert({
  where: { id: "local-device-001" },
  update: {
    displayName: "Synthetic Windows 11 device",
    environment: "development",
  },
  create: {
    id: "local-device-001",
    displayName: "Synthetic Windows 11 device",
    environment: "development",
  },
});

await prisma.$disconnect();
