namespace ITSupportNative.Catalog.Domain;

internal static class CatalogValue
{
    public static string NormalizeRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
