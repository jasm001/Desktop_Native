import type { CatalogProduct } from "@it-support-native/control-plane-contracts";

export const syntheticCatalog: readonly CatalogProduct[] = [
  {
    id: "secure-transfer",
    name: "Secure Transfer",
    version: "6.5",
    status: "approved",
    actionId: "software.install.simulated.v1",
  },
  {
    id: "share-anywhere",
    name: "Share Anywhere",
    version: "3.1",
    status: "prohibited",
    actionId: null,
  },
];
