using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Domain;
using ITSupportNative.Catalog.Fixtures;

namespace ITSupportNative.UnitTests;

public sealed class CatalogSearchServiceTests
{
    private readonly CatalogSearchService _service = new(SyntheticCatalog.Products);

    [Fact]
    public void EmptySearchReturnsEveryProductInStableNameOrder()
    {
        IReadOnlyList<SoftwareProduct> results = _service.Search(new());

        Assert.Equal(SyntheticCatalog.Products.Count, results.Count);
        Assert.Equal(
            results.OrderBy(product => product.Name, StringComparer.OrdinalIgnoreCase),
            results);
    }

    [Theory]
    [InlineData("aurora", "aurora-code")]
    [InlineData("SFTP CLIENT", "secure-transfer")]
    [InlineData("transfer", "legacy-transfer", "secure-transfer", "share-anywhere")]
    [InlineData("analytics", "insight-studio")]
    public void SearchMatchesNormalizedFields(string text, params string[] expectedIds)
    {
        IReadOnlyList<SoftwareProduct> results = _service.Search(new(text));

        Assert.Equal(expectedIds, results.Select(product => product.Id));
    }

    [Fact]
    public void SearchIgnoresDiacritics()
    {
        var product = new SoftwareProduct(
            "analysis",
            "Análisis",
            "Example",
            "Data",
            new SoftwareVersion("1"),
            new SoftwareLicense(SoftwareLicenseType.OpenSource, "Approved"),
            SoftwareProductStatus.Approved);
        var service = new CatalogSearchService([product]);

        SoftwareProduct result = Assert.Single(service.Search(new("analisis")));

        Assert.Equal("analysis", result.Id);
    }

    [Fact]
    public void FiltersByStatusAndLicenseTogether()
    {
        IReadOnlyList<SoftwareProduct> results = _service.Search(
            new(
                Status: SoftwareProductStatus.Approved,
                LicenseType: SoftwareLicenseType.OpenSource));

        Assert.Equal(
            ["aurora-code", "secure-transfer"],
            results.Select(product => product.Id));
    }

    [Fact]
    public void SearchReturnsEmptyWhenFiltersDoNotIntersect()
    {
        IReadOnlyList<SoftwareProduct> results = _service.Search(
            new(
                Status: SoftwareProductStatus.Prohibited,
                LicenseType: SoftwareLicenseType.Commercial));

        Assert.Empty(results);
    }
}
