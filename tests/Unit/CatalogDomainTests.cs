using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.UnitTests;

public sealed class CatalogDomainTests
{
    [Fact]
    public void ProductNormalizesRequiredValuesAndAliases()
    {
        var product = new SoftwareProduct(
            " product-id ",
            " Product ",
            " Publisher ",
            " Category ",
            new SoftwareVersion(" 1.0 "),
            new SoftwareLicense(SoftwareLicenseType.Freeware, " Allowed "),
            SoftwareProductStatus.Approved,
            aliases: [" Alias ", "alias"]);

        Assert.Equal("product-id", product.Id);
        Assert.Equal("Product", product.Name);
        Assert.Equal("1.0", product.Version.DisplayName);
        Assert.Equal("Allowed", product.License.Usage);
        Assert.Equal(["Alias"], product.Aliases);
    }

    [Fact]
    public void ProductRejectsMissingIdentity()
    {
        Assert.Throws<ArgumentException>(
            () => new SoftwareProduct(
                " ",
                "Product",
                "Publisher",
                "Category",
                new SoftwareVersion("1.0"),
                new SoftwareLicense(SoftwareLicenseType.Freeware, "Allowed"),
                SoftwareProductStatus.Approved));
    }

    [Theory]
    [InlineData(SoftwareLicenseType.OpenSource, false)]
    [InlineData(SoftwareLicenseType.Freeware, false)]
    [InlineData(SoftwareLicenseType.Commercial, true)]
    public void LicenseIdentifiesEntitlementRequirement(
        SoftwareLicenseType type,
        bool expected)
    {
        var license = new SoftwareLicense(type, "Synthetic usage");

        Assert.Equal(expected, license.RequiresEntitlement);
    }
}
