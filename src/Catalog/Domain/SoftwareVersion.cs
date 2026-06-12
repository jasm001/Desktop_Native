namespace ITSupportNative.Catalog.Domain;

public sealed record SoftwareVersion
{
    public SoftwareVersion(string displayName)
    {
        DisplayName = CatalogValue.NormalizeRequired(displayName, nameof(displayName));
    }

    public string DisplayName { get; }
}
