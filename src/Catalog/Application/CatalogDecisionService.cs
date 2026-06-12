using System.Collections.ObjectModel;
using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.Catalog.Application;

public sealed class CatalogDecisionService
{
    private readonly IReadOnlyList<SoftwareProduct> _products;

    public CatalogDecisionService(IReadOnlyList<SoftwareProduct> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products = new ReadOnlyCollection<SoftwareProduct>(products.ToArray());
    }

    public CatalogDecision Evaluate(string productReference, CatalogDecisionPurpose purpose)
    {
        string normalizedReference = CatalogText.Normalize(productReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedReference, nameof(productReference));

        SoftwareProduct? product = _products.FirstOrDefault(
            candidate => IsReferenceMatch(candidate, normalizedReference));

        if (product is null)
        {
            return purpose == CatalogDecisionPurpose.Information
                ? CreateUnknownDecision(CatalogDecisionKind.Inform)
                : CreateUnknownDecision(CatalogDecisionKind.Escalate);
        }

        IReadOnlyList<SoftwareProduct> alternatives = ResolveAlternatives(product);

        if (purpose == CatalogDecisionPurpose.Information)
        {
            return new(
                CatalogDecisionKind.Inform,
                product.Status,
                "The query returns catalog information only and creates no action.",
                product,
                alternatives);
        }

        return product.Status switch
        {
            SoftwareProductStatus.Prohibited => new(
                CatalogDecisionKind.Reject,
                product.Status,
                "Prohibited software cannot be proposed or acquired.",
                product,
                alternatives),
            SoftwareProductStatus.EndOfLife => new(
                CatalogDecisionKind.Reject,
                product.Status,
                "End-of-life software is rejected by default.",
                product,
                alternatives),
            SoftwareProductStatus.Unlisted => new(
                CatalogDecisionKind.Escalate,
                product.Status,
                "Unlisted software requires review before any proposal.",
                product,
                alternatives),
            SoftwareProductStatus.Approved when product.License.RequiresEntitlement => new(
                CatalogDecisionKind.Escalate,
                product.Status,
                "Commercial software requires a license or purchasing review.",
                product,
                alternatives),
            SoftwareProductStatus.Approved => new(
                CatalogDecisionKind.Propose,
                product.Status,
                "Approved software may be proposed for explicit confirmation.",
                product,
                alternatives),
            _ => throw new InvalidOperationException("Unsupported catalog status."),
        };
    }

    private static bool IsReferenceMatch(SoftwareProduct product, string normalizedReference)
    {
        return CatalogText.Normalize(product.Id) == normalizedReference
            || CatalogText.Normalize(product.Name) == normalizedReference
            || product.Aliases.Any(alias => CatalogText.Normalize(alias) == normalizedReference);
    }

    private static CatalogDecision CreateUnknownDecision(CatalogDecisionKind kind)
    {
        string rationale = kind == CatalogDecisionKind.Inform
            ? "The product is not listed; the query creates no action."
            : "Unknown products are treated as unlisted and require review.";

        return new(
            kind,
            SoftwareProductStatus.Unlisted,
            rationale,
            Product: null,
            Alternatives: Array.Empty<SoftwareProduct>());
    }

    private ReadOnlyCollection<SoftwareProduct> ResolveAlternatives(SoftwareProduct product)
    {
        SoftwareProduct[] alternatives = product.Alternatives
            .Select(alternative => _products.FirstOrDefault(
                candidate => string.Equals(
                    candidate.Id,
                    alternative.ProductId,
                    StringComparison.OrdinalIgnoreCase)))
            .OfType<SoftwareProduct>()
            .ToArray();

        return new ReadOnlyCollection<SoftwareProduct>(alternatives);
    }
}
