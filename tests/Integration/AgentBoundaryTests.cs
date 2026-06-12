using ITSupportNative.DeviceAgent.Diagnostics;
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

    [Fact]
    public void DiagnosticUseCaseDoesNotDependOnJobPersistence()
    {
        Type[] constructorDependencies = typeof(AgentDiagnosticsService)
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        Assert.DoesNotContain(typeof(IAgentJobStore), constructorDependencies);
        Assert.DoesNotContain(typeof(SqliteAgentJobStore), constructorDependencies);
    }
}
