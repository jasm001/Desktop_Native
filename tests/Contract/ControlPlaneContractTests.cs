using System.Text;
using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.ContractTests;

public sealed class ControlPlaneContractTests
{
    [Fact]
    public void SerializeUsesCanonicalUtcTimestamps()
    {
        var request = new ReportAgentJobResultRequest(
            Guid.NewGuid().ToString(),
            "succeeded",
            [
                new(
                    "job.simulation.verified",
                    new DateTimeOffset(2026, 6, 13, 18, 30, 0, TimeSpan.Zero)),
            ]);

        string json = Encoding.UTF8.GetString(ControlPlaneJson.Serialize(request));

        Assert.Contains(
            "\"recordedAt\":\"2026-06-13T18:30:00Z\"",
            json,
            StringComparison.Ordinal);
        Assert.DoesNotContain("+00:00", json, StringComparison.Ordinal);
    }
}
