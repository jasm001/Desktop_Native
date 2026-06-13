using System.Net.Http.Headers;
using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.Desktop.ControlPlane;

public sealed class HttpControlPlaneRequestClient : IControlPlaneRequestClient
{
    private const string DeviceId = "local-device-001";
    private const string SimulatedActionId = "software.install.simulated.v1";

    private readonly HttpClient _httpClient;

    public HttpControlPlaneRequestClient(
        HttpClient httpClient,
        ControlPlaneClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _httpClient.BaseAddress = options.BaseAddress;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<CreateSoftwareInstallationData?> CreateSoftwareInstallationAsync(
        string idempotencyKey,
        string productId,
        string productVersion,
        CancellationToken cancellationToken)
    {
        var body = new CreateSoftwareInstallationRequest(
            Confirmed: true,
            DeviceId,
            productId,
            productVersion,
            SimulatedActionId);
        using var request = CreateRequest(
            HttpMethod.Post,
            "api/v1/requests/software-installations",
            $"desktop-{Guid.NewGuid():N}",
            idempotencyKey,
            body);
        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);
        return await ReadDataAsync<CreateSoftwareInstallationData>(
            response,
            cancellationToken);
    }

    public async Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
        string requestId,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(
            HttpMethod.Get,
            $"api/v1/requests/{Uri.EscapeDataString(requestId)}",
            $"desktop-status-{Guid.NewGuid():N}");
        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);
        GetSupportRequestData? data =
            await ReadDataAsync<GetSupportRequestData>(
                response,
                cancellationToken);
        return data?.Request;
    }

    private static HttpRequestMessage CreateRequest<TBody>(
        HttpMethod method,
        string path,
        string correlationId,
        string idempotencyKey,
        TBody body)
    {
        HttpRequestMessage request = CreateRequest(method, path, correlationId);
        request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Content = new ByteArrayContent(ControlPlaneJson.Serialize(body));
        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return request;
    }

    private static HttpRequestMessage CreateRequest(
        HttpMethod method,
        string path,
        string correlationId)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("X-Correlation-Id", correlationId);
        return request;
    }

    private static async Task<TData?> ReadDataAsync<TData>(
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
