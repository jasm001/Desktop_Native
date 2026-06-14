using System.Net;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Desktop.ControlPlane;

namespace ITSupportNative.WindowsUi.Tests;

public sealed class HttpControlPlaneRequestClientTests
{
    [Fact]
    public async Task CreatePropagatesCorrelationDeviceAndIdempotency()
    {
        var handler = new RecordingHandler(
            async request =>
            {
                CreateSoftwareInstallationRequest? body =
                    ControlPlaneJson.Deserialize<CreateSoftwareInstallationRequest>(
                        await request.Content!.ReadAsByteArrayAsync());
                Assert.Equal("local-device-001", body?.DeviceId);
                Assert.Equal(
                    "channel-correlation-1",
                    request.Headers.GetValues("X-Correlation-Id").Single());
                Assert.Equal(
                    "channel-idempotency-1",
                    request.Headers.GetValues("Idempotency-Key").Single());

                return JsonResponse(
                    new ControlPlaneApiEnvelope<CreateSoftwareInstallationData>(
                        new(
                            CreateRequest("channel-correlation-1"),
                            Replayed: false),
                        new("v1", "channel-correlation-1")),
                    HttpStatusCode.Created);
            });
        var client = CreateClient(handler);

        CreateSoftwareInstallationData? result =
            await client.CreateSoftwareInstallationAsync(
                "channel-correlation-1",
                "channel-idempotency-1",
                "local-device-001",
                "secure-transfer",
                "6.5",
                CancellationToken.None);

        Assert.False(result?.Replayed);
        Assert.Equal("secure-transfer", result?.Request.ProductId);
    }

    [Fact]
    public async Task CaseStatusUsesTheTypedReadOnlyEndpoint()
    {
        const string requestId = "11111111-1111-4111-8111-111111111111";
        var handler = new RecordingHandler(
            request =>
            {
                Assert.Equal(
                    $"/api/v1/requests/{requestId}/case",
                    request.RequestUri?.AbsolutePath);
                Assert.Equal(HttpMethod.Get, request.Method);
                return Task.FromResult(
                    JsonResponse(
                        new ControlPlaneApiEnvelope<GetBotCaseData>(
                            new(
                                new(
                                    "33333333-3333-4333-8333-333333333333",
                                    requestId,
                                    "channel-correlation-2",
                                    "software_installation",
                                    "escalated",
                                    "failed",
                                    WaitingForUserSince: null,
                                    EscalatedAt: DateTimeOffset.UtcNow,
                                    DateTimeOffset.UtcNow,
                                    DateTimeOffset.UtcNow,
                                    ExternalTicket: null)),
                            new("v1", "channel-correlation-2")),
                        HttpStatusCode.OK));
            });
        var client = CreateClient(handler);

        ControlPlaneBotCase? result = await client.GetBotCaseAsync(
            "channel-correlation-2",
            requestId,
            CancellationToken.None);

        Assert.Equal("escalated", result?.Status);
        Assert.Equal(requestId, result?.RequestId);
        Assert.Equal(1, handler.CallCount);
    }

    private static HttpControlPlaneRequestClient CreateClient(
        HttpMessageHandler handler)
    {
        return new(
            new HttpClient(handler),
            new ControlPlaneClientOptions(new Uri("http://localhost/")));
    }

    private static HttpResponseMessage JsonResponse<T>(
        T value,
        HttpStatusCode statusCode)
    {
        return new(statusCode)
        {
            Content = new ByteArrayContent(ControlPlaneJson.Serialize(value)),
        };
    }

    private static ControlPlaneSupportRequest CreateRequest(
        string correlationId)
    {
        return new(
            "11111111-1111-4111-8111-111111111111",
            "REQ-1111111111111111",
            correlationId,
            "confirmed",
            "local-device-001",
            "secure-transfer",
            "6.5",
            "software.install.simulated.v1",
            DateTimeOffset.UtcNow,
            new(
                "22222222-2222-4222-8222-222222222222",
                "queued",
                []));
    }

    private sealed class RecordingHandler(
        Func<HttpRequestMessage, Task<HttpResponseMessage>> responder)
        : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            return responder(request);
        }
    }
}
