using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentBoundaryTests
{
    private static readonly string[] ForbiddenAssemblyNames =
    [
        "ITSupportNative.Desktop",
        "Microsoft.UI.Xaml",
        "Microsoft.WinUI",
    ];

    [Fact]
    public void AgentCoreDoesNotReferenceUserInterfaceAssemblies()
    {
        string[] references = typeof(AgentJobService)
            .Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .OfType<string>()
            .ToArray();

        Assert.DoesNotContain(references, ForbiddenAssemblyNames.Contains);
    }
}
