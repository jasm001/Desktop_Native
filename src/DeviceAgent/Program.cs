using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.Diagnostics;
using ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;
using ITSupportNative.DeviceAgent.Execution;
using ITSupportNative.DeviceAgent.Ipc;
using ITSupportNative.DeviceAgent.Jobs;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .Configure<DeviceAgentOptions>(
        builder.Configuration.GetSection(DeviceAgentOptions.SectionName))
    .AddSingleton(TimeProvider.System)
    .AddSingleton(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new AgentActionAuthorizationPolicy(options.ExecutionProfile);
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
    .AddSingleton<AgentRequestDispatcher>()
    .AddHostedService<AgentJobWorker>()
    .AddHostedService<NamedPipeAgentWorker>();

var host = builder.Build();
await host.RunAsync();
