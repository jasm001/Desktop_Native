using System.Net.Http.Headers;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.DeviceAgent.Configuration;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.ControlPlane;

public sealed class HttpControlPlaneAgentClient : IControlPlaneAgentClient
{
    private readonly HttpClient _httpClient;
    private readonly DeviceAgentOptions _options;

    public HttpControlPlaneAgentClient(
        HttpClient httpClient,
        IOptions<DeviceAgentOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        if (!Uri.TryCreate(
                _options.ControlPlaneBaseUrl,
                UriKind.Absolute,
                out Uri? baseAddress)
            || !baseAddress.IsLoopback
            || baseAddress.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException(
                "The local control plane URL must be an HTTP loopback address.");
        }

        _httpClient = httpClient;
        _httpClient.BaseAddress = baseAddress;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<ClaimedAgentJob?> ClaimNextAsync(
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = CreateRequest(
            HttpMethod.Post,
            "api/v1/agent/jobs/claim",
            $"agent-claim-{Guid.NewGuid():N}");
        request.Content = JsonContent(
            new ClaimAgentJobRequest(_options.ControlPlaneDeviceId));

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);
        ClaimAgentJobData data = await ReadDataAsync<ClaimAgentJobData>(
            response,
            cancellationToken);
        return data.Job;
    }

    public async Task<ReportAgentJobResultData> ReportResultAsync(
        ClaimedAgentJob job,
        string result,
        IReadOnlyList<ReportAgentEvidence> evidence,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = CreateRequest(
            HttpMethod.Post,
            $"api/v1/agent/jobs/{Uri.EscapeDataString(job.JobId)}/result",
            $"agent-result-{Guid.NewGuid():N}");
        request.Headers.Add("Idempotency-Key", $"agent-result:{job.JobId}");
        request.Content = JsonContent(
            new ReportAgentJobResultRequest(
                job.ClaimToken,
                result,
                evidence));

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);
        return await ReadDataAsync<ReportAgentJobResultData>(
            response,
            cancellationToken);
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string path,
        string correlationId)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("X-Correlation-Id", correlationId);
        request.Headers.Add(
            "X-Development-Agent-Id",
            _options.ControlPlaneAgentId);
        return request;
    }

    private static ByteArrayContent JsonContent<T>(T value)
    {
        var content = new ByteArrayContent(ControlPlaneJson.Serialize(value));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return content;
    }

    private static async Task<TData> ReadDataAsync<TData>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        byte[] content = await response.Content.ReadAsByteArrayAsync(
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Control plane returned HTTP {(int)response.StatusCode}.",
                inner: null,
                response.StatusCode);
        }

        ControlPlaneApiEnvelope<TData>? envelope =
            ControlPlaneJson.Deserialize<ControlPlaneApiEnvelope<TData>>(content);
        return envelope is null
            ? throw new InvalidDataException("The control plane response was invalid.")
            : envelope.Data;
    }
}
