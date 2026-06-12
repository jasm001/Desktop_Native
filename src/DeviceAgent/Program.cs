using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.Ipc;
using ITSupportNative.DeviceAgent.Jobs;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .Configure<DeviceAgentOptions>(
        builder.Configuration.GetSection(DeviceAgentOptions.SectionName))
    .AddSingleton(TimeProvider.System)
    .AddSingleton<AgentActionAuthorizationPolicy>()
    .AddSingleton<IAgentJobStore>(services =>
    {
        var options = services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<DeviceAgentOptions>>()
            .Value;
        return new SqliteAgentJobStore(options.StateFilePath);
    })
    .AddSingleton<AgentJobService>()
    .AddSingleton<AgentRequestDispatcher>()
    .AddHostedService<SimulatedJobWorker>()
    .AddHostedService<NamedPipeAgentWorker>();

var host = builder.Build();
await host.RunAsync();
