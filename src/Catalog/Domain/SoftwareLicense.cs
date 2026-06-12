namespace ITSupportNative.Catalog.Domain;

public sealed record SoftwareLicense
{
    public SoftwareLicense(SoftwareLicenseType type, string usage)
    {
        Type = type;
        Usage = CatalogValue.NormalizeRequired(usage, nameof(usage));
    }

    public SoftwareLicenseType Type { get; }

    public string Usage { get; }

    public bool RequiresEntitlement => Type == SoftwareLicenseType.Commercial;
}
