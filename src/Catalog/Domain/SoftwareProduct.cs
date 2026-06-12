using System.Collections.ObjectModel;

namespace ITSupportNative.Catalog.Domain;

public sealed class SoftwareProduct
{
    public SoftwareProduct(
        string id,
        string name,
        string publisher,
        string category,
        SoftwareVersion version,
        SoftwareLicense license,
        SoftwareProductStatus status,
        IEnumerable<string>? aliases = null,
        IEnumerable<ProductAlternative>? alternatives = null)
    {
        Id = CatalogValue.NormalizeRequired(id, nameof(id));
        Name = CatalogValue.NormalizeRequired(name, nameof(name));
        Publisher = CatalogValue.NormalizeRequired(publisher, nameof(publisher));
        Category = CatalogValue.NormalizeRequired(category, nameof(category));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        License = license ?? throw new ArgumentNullException(nameof(license));
        Status = status;
        Aliases = NormalizeAliases(aliases);
        Alternatives = new ReadOnlyCollection<ProductAlternative>(
            alternatives?.ToArray() ?? []);
    }

    public string Id { get; }

    public string Name { get; }

    public string Publisher { get; }

    public string Category { get; }

    public SoftwareVersion Version { get; }

    public SoftwareLicense License { get; }

    public SoftwareProductStatus Status { get; }

    public IReadOnlyList<string> Aliases { get; }

    public IReadOnlyList<ProductAlternative> Alternatives { get; }

    private static ReadOnlyCollection<string> NormalizeAliases(IEnumerable<string>? aliases)
    {
        string[] normalized = aliases?
            .Select(alias => CatalogValue.NormalizeRequired(alias, nameof(aliases)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

        return new ReadOnlyCollection<string>(normalized);
    }
}
