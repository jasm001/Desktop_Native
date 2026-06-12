using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.Catalog.Application;

public sealed record CatalogDecision(
    CatalogDecisionKind Kind,
    SoftwareProductStatus EffectiveStatus,
    string Rationale,
    SoftwareProduct? Product,
    IReadOnlyList<SoftwareProduct> Alternatives);
