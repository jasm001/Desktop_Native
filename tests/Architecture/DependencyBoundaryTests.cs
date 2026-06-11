using ITSupportNative.BuildingBlocks;
using ITSupportNative.Contracts;

namespace ITSupportNative.ArchitectureTests;

public sealed class DependencyBoundaryTests
{
    private static readonly string[] ForbiddenAssemblyNames =
    [
        "ITSupportNative.Desktop",
        "ITSupportNative.DeviceAgent",
    ];

    [Theory]
    [InlineData(typeof(ProductInfo))]
    [InlineData(typeof(ContractVersion))]
    public void FoundationalAssembliesDoNotReferenceExecutableBoundaries(Type assemblyMarker)
    {
        var references = assemblyMarker.Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .OfType<string>();

        Assert.DoesNotContain(references, ForbiddenAssemblyNames.Contains);
    }
}
