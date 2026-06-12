using System.Collections.ObjectModel;
using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.Catalog.Fixtures;

public static class SyntheticCatalog
{
    public static IReadOnlyList<SoftwareProduct> Products { get; } =
        new ReadOnlyCollection<SoftwareProduct>(
        [
            new(
                "archive-lite",
                "Archive Lite",
                "Example Tools",
                "Utilities",
                new SoftwareVersion("24.09"),
                new SoftwareLicense(SoftwareLicenseType.Freeware, "Free for managed endpoints"),
                SoftwareProductStatus.Approved,
                aliases: ["Archive"]),
            new(
                "aurora-code",
                "Aurora Code Editor",
                "Example Foundation",
                "Development",
                new SoftwareVersion("1.100"),
                new SoftwareLicense(SoftwareLicenseType.OpenSource, "Approved open-source license"),
                SoftwareProductStatus.Approved,
                aliases: ["Aurora", "Code Editor"]),
            new(
                "diagram-lab",
                "Diagram Lab",
                "Example Labs",
                "Design",
                new SoftwareVersion("5.2"),
                new SoftwareLicense(SoftwareLicenseType.Freeware, "License not yet reviewed"),
                SoftwareProductStatus.Unlisted),
            new(
                "insight-studio",
                "Insight Studio",
                "Example Analytics",
                "Data",
                new SoftwareVersion("2026.05"),
                new SoftwareLicense(SoftwareLicenseType.Commercial, "Named-user subscription"),
                SoftwareProductStatus.Approved),
            new(
                "legacy-transfer",
                "Legacy Transfer Client",
                "Example Legacy",
                "File transfer",
                new SoftwareVersion("8.0"),
                new SoftwareLicense(SoftwareLicenseType.Freeware, "Legacy freeware terms"),
                SoftwareProductStatus.EndOfLife,
                alternatives:
                [
                    new ProductAlternative("secure-transfer", "Supported secure transfer client"),
                ]),
            new(
                "secure-transfer",
                "Secure Transfer",
                "Example Foundation",
                "File transfer",
                new SoftwareVersion("6.5"),
                new SoftwareLicense(SoftwareLicenseType.OpenSource, "Approved open-source license"),
                SoftwareProductStatus.Approved,
                aliases: ["SFTP Client"]),
            new(
                "share-anywhere",
                "Share Anywhere",
                "Example Networks",
                "File transfer",
                new SoftwareVersion("3.1"),
                new SoftwareLicense(SoftwareLicenseType.Freeware, "Consumer freeware terms"),
                SoftwareProductStatus.Prohibited,
                alternatives:
                [
                    new ProductAlternative("secure-transfer", "Approved managed alternative"),
                ]),
        ]);
}
