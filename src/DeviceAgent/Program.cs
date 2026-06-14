using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.ControlPlane;
using ITSupportNative.DeviceAgent.Diagnostics;
using ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;
using ITSupportNative.DeviceAgent.Execution;
using ITSupportNative.DeviceAgent.Ipc;
using ITSupportNative.DeviceAgent.Jobs;

var builder = Host.CreateApplicationBuilder(args);
var deviceAgentConfiguration = builder.Configuration.GetSection(
    DeviceAgentOptions.SectionName);
var startupOptions = deviceAgentConfiguration.Get<DeviceAgentOptions>()
    ?? new DeviceAgentOptions();
if (!DeviceAgentConfigurationPolicy.IsValid(
        startupOptions,
        builder.Environment.EnvironmentName))
{
    Console.Error.WriteLine("The DeviceAgent configuration is invalid.");
    Environment.ExitCode = 78;
    return;
}

builder.Services
    .Configure<DeviceAgentOptions>(
        deviceAgentConfiguration)
    .AddOptions<DeviceAgentOptions>()
    .Validate(
        options => DeviceAgentConfigurationPolicy.IsValid(
            options,
            builder.Environment.EnvironmentName),
        "The DeviceAgent configuration is not valid for this environment.")
    .ValidateOnStart();

builder.Services
    .AddSingleton(TimeProvider.System)
    .AddSingleton(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new AgentActionAuthorizationPolicy(options.ExecutionProfile);
    })
    .AddSingleton(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new AgentJobExecutionGate(options.JobExecutionEnabled);
    })
    .AddSingleton<IAgentJobStore>(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new SqliteAgentJobStore(options.StateFilePath);
    })
    .AddSingleton<ISoftwareArtifactSource>(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new LocalDevelopmentArtifactSource(options.LocalArtifactRootPath);
    })
    .AddSingleton<ISoftwareProductDetector, WindowsInstallerProductDetector>()
    .AddSingleton<IProcessRunner, WindowsProcessRunner>()
    .AddSingleton<IExecutionPlatform, WindowsX64ExecutionPlatform>()
    .AddSingleton<ISoftwareExecutionAdapter>(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new SevenZip2601X64Adapter(
            options.ExecutionProfile,
            services.GetRequiredService<ISoftwareArtifactSource>(),
            services.GetRequiredService<ISoftwareProductDetector>(),
            services.GetRequiredService<IProcessRunner>(),
            services.GetRequiredService<IExecutionPlatform>());
    })
    .AddSingleton<IAgentActionExecutor, SoftwareAgentActionExecutor>()
    .AddSingleton<AgentJobService>()
    .AddSingleton<IWindowsDiagnosticCollector, WindowsDiagnosticCollector>()
    .AddSingleton<IStorageDiagnosticCollector, StorageDiagnosticCollector>()
    .AddSingleton<IMemoryDiagnosticCollector, MemoryDiagnosticCollector>()
    .AddSingleton<IDomainReachabilityProbe, WindowsDomainReachabilityProbe>()
    .AddSingleton<INetworkDiagnosticCollector, NetworkDiagnosticCollector>()
    .AddSingleton<IAgentVersionDiagnosticCollector, AgentVersionDiagnosticCollector>()
    .AddSingleton<AgentDiagnosticsService>()
    .AddSingleton<AgentRequestDispatcher>();

builder.Services
    .AddSingleton<ControlPlaneAgentSyncService>()
    .AddHostedService<AgentJobWorker>()
    .AddHostedService<ControlPlaneAgentWorker>()
    .AddHostedService<NamedPipeAgentWorker>();
builder.Services.AddHttpClient<IControlPlaneAgentClient, HttpControlPlaneAgentClient>();

var host = builder.Build();
await host.RunAsync();
