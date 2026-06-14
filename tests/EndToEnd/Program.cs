using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Desktop.ControlPlane;
using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.ControlPlane;
using ITSupportNative.DeviceAgent.Jobs;
using Microsoft.Extensions.Options;

string? controlPlaneUrl =
    Environment.GetEnvironmentVariable("IT_SUPPORT_CONTROL_PLANE_URL");
if (!Uri.TryCreate(controlPlaneUrl, UriKind.Absolute, out Uri? baseAddress)
    || !baseAddress.IsLoopback)
{
    throw new InvalidOperationException(
        "IT_SUPPORT_CONTROL_PLANE_URL must be an absolute loopback URL.");
}

string stateDirectory = Path.Combine(
    Path.GetTempPath(),
    "ITSupportNative.EndToEnd",
    Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(stateDirectory);

try
{
    var desktopClient = new HttpControlPlaneRequestClient(
        new HttpClient(),
        new ControlPlaneClientOptions(baseAddress));
    string idempotencyKey = $"desktop-e2e-{Guid.NewGuid():N}";
    CreateSoftwareInstallationData created =
        await desktopClient.CreateSoftwareInstallationAsync(
            $"desktop-e2e-correlation-{Guid.NewGuid():N}",
            idempotencyKey,
            "local-device-001",
            "secure-transfer",
            "6.5",
            CancellationToken.None)
        ?? throw new InvalidOperationException(
            "The desktop control plane client was unexpectedly disabled.");

    var agentOptions = Options.Create(
        new DeviceAgentOptions
        {
            ControlPlaneBaseUrl = baseAddress.ToString(),
            ControlPlaneDeviceId = "local-device-001",
            ControlPlaneAgentId = "local-agent-001",
        });
    var agentClient = new HttpControlPlaneAgentClient(
        new HttpClient(),
        agentOptions);
    using var jobs = new AgentJobService(
        new SqliteAgentJobStore(Path.Combine(stateDirectory, "jobs.db")),
        new AgentActionAuthorizationPolicy(),
        TimeProvider.System);
    var synchronization = new ControlPlaneAgentSyncService(agentClient, jobs);

    ControlPlaneSupportRequest? completed = null;
    for (int attempt = 0; attempt < 30 && completed is null; attempt++)
    {
        completed = await synchronization.RunOnceAsync(CancellationToken.None);
        if (completed is null)
        {
            await Task.Delay(250);
        }
    }

    if (completed?.Status != "completed"
        || completed.Job.Status != "completed"
        || completed.Job.Evidence.Count != 3)
    {
        throw new InvalidOperationException(
            "The simulated agent did not complete the control plane request.");
    }

    ControlPlaneSupportRequest persisted =
        await desktopClient.GetSupportRequestAsync(
            $"desktop-e2e-status-{Guid.NewGuid():N}",
            created.Request.Id,
            CancellationToken.None)
        ?? throw new InvalidOperationException(
            "The desktop client could not read the completed request.");
    if (persisted.Status != "completed"
        || persisted.Job.Evidence.All(
            item => item.Code != "job.simulation.verified"))
    {
        throw new InvalidOperationException(
            "The final request did not expose sanitized agent evidence.");
    }

    Console.WriteLine(
        $"Control plane E2E passed for {persisted.Reference} with "
        + $"{persisted.Job.Evidence.Count} evidence records.");
}
finally
{
    if (Directory.Exists(stateDirectory))
    {
        Directory.Delete(stateDirectory, recursive: true);
    }
}
