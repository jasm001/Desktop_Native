import { PrismaPg } from "@prisma/adapter-pg";
import { PrismaClient } from "../../generated/prisma/client";

let prisma: PrismaClient | undefined;

export function getPrisma(): PrismaClient {
  if (prisma !== undefined) {
    return prisma;
  }

  const connectionString = process.env.DATABASE_URL;
  if (connectionString === undefined || connectionString.trim().length === 0) {
    throw new Error("database_configuration_unavailable");
  }

  const adapter = new PrismaPg({
    connectionString,
    connectionTimeoutMillis: 5_000,
    idleTimeoutMillis: 10_000,
    max: 10,
  });

  prisma = new PrismaClient({ adapter });
  return prisma;
}

export async function disconnectPrisma(): Promise<void> {
  if (prisma !== undefined) {
    await prisma.$disconnect();
    prisma = undefined;
  }
}
