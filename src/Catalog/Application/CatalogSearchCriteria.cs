using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.Catalog.Application;

public sealed record CatalogSearchCriteria(
    string? SearchText = null,
    SoftwareProductStatus? Status = null,
    SoftwareLicenseType? LicenseType = null);
