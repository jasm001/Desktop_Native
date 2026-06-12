using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Domain;
using ITSupportNative.Catalog.Fixtures;

namespace ITSupportNative.UnitTests;

public sealed class CatalogDecisionServiceTests
{
    private readonly CatalogDecisionService _service = new(SyntheticCatalog.Products);

    [Theory]
    [InlineData("aurora-code", CatalogDecisionKind.Propose)]
    [InlineData("insight-studio", CatalogDecisionKind.Escalate)]
    [InlineData("diagram-lab", CatalogDecisionKind.Escalate)]
    [InlineData("legacy-transfer", CatalogDecisionKind.Reject)]
    [InlineData("share-anywhere", CatalogDecisionKind.Reject)]
    public void AcquisitionUsesDeterministicCatalogRules(
        string productReference,
        CatalogDecisionKind expected)
    {
        CatalogDecision decision = _service.Evaluate(
            productReference,
            CatalogDecisionPurpose.Acquisition);

        Assert.Equal(expected, decision.Kind);
    }

    [Fact]
    public void InformationNeverCreatesAnActionDecision()
    {
        CatalogDecision decision = _service.Evaluate(
            "Share Anywhere",
            CatalogDecisionPurpose.Information);

        Assert.Equal(CatalogDecisionKind.Inform, decision.Kind);
        Assert.Equal(SoftwareProductStatus.Prohibited, decision.EffectiveStatus);
        Assert.NotNull(decision.Product);
    }

    [Fact]
    public void UnknownAcquisitionIsTreatedAsUnlistedAndEscalated()
    {
        CatalogDecision decision = _service.Evaluate(
            "Unknown Product",
            CatalogDecisionPurpose.Acquisition);

        Assert.Equal(CatalogDecisionKind.Escalate, decision.Kind);
        Assert.Equal(SoftwareProductStatus.Unlisted, decision.EffectiveStatus);
        Assert.Null(decision.Product);
    }

    [Fact]
    public void UnknownInformationOnlyInforms()
    {
        CatalogDecision decision = _service.Evaluate(
            "Unknown Product",
            CatalogDecisionPurpose.Information);

        Assert.Equal(CatalogDecisionKind.Inform, decision.Kind);
        Assert.Equal(SoftwareProductStatus.Unlisted, decision.EffectiveStatus);
    }

    [Theory]
    [InlineData("Aurora")]
    [InlineData("AURORA CODE EDITOR")]
    [InlineData("aurora-code")]
    public void ProductCanBeResolvedByAliasNameOrId(string productReference)
    {
        CatalogDecision decision = _service.Evaluate(
            productReference,
            CatalogDecisionPurpose.Acquisition);

        Assert.Equal("aurora-code", decision.Product?.Id);
    }

    [Theory]
    [InlineData("legacy-transfer")]
    [InlineData("share-anywhere")]
    public void RejectionReturnsApprovedAlternative(string productReference)
    {
        CatalogDecision decision = _service.Evaluate(
            productReference,
            CatalogDecisionPurpose.Acquisition);

        SoftwareProduct alternative = Assert.Single(decision.Alternatives);
        Assert.Equal("secure-transfer", alternative.Id);
        Assert.Equal(SoftwareProductStatus.Approved, alternative.Status);
    }
}
