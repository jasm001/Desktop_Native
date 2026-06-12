namespace ITSupportNative.Catalog.Domain;

public sealed record ProductAlternative
{
    public ProductAlternative(string productId, string reason)
    {
        ProductId = CatalogValue.NormalizeRequired(productId, nameof(productId));
        Reason = CatalogValue.NormalizeRequired(reason, nameof(reason));
    }

    public string ProductId { get; }

    public string Reason { get; }
}
