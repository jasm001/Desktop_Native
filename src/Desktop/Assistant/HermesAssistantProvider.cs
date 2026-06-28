using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ITSupportNative.Desktop.Assistant;

public sealed class HermesAssistantProvider(
    HttpClient httpClient,
    HermesAssistantOptions options)
    : IAssistantProvider
{
    private const int MaximumMessageLength = 1_000;

    public bool IsAvailable => true;

    public async Task<AssistantProviderReply> GetResponseAsync(
        string message,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        string boundedMessage = message.Trim();
        if (boundedMessage.Length > MaximumMessageLength)
        {
            boundedMessage = boundedMessage[..MaximumMessageLength];
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(options.BaseEndpoint, "chat/completions"));
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            options.ApiKey);
        request.Content = JsonContent.Create(
            new ChatCompletionRequest(
                options.Model,
                [
                    new(
                        "system",
                        "Eres el asistente local de demostracion de IT Support Native. Responde solo con orientacion informativa, no pidas credenciales, no generes comandos, scripts, rutas, argumentos ni instrucciones operativas privilegiadas. Si el usuario pide ejecutar una accion, indica que debe usarse el flujo tipado y confirmado de la app."),
                    new("user", boundedMessage),
                ],
                Temperature: 0.2,
                MaxTokens: 500));

        using CancellationTokenSource timeout =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Timeout);

        using HttpResponseMessage response =
            await httpClient.SendAsync(request, timeout.Token);
        if (!response.IsSuccessStatusCode)
        {
            return new(
                "Hermes local no devolvio una respuesta disponible. El asistente conserva las opciones deterministas.",
                "hermes-unavailable");
        }

        ChatCompletionResponse? payload =
            await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
                cancellationToken: timeout.Token);
        string? text = payload?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(text))
        {
            return new(
                "Hermes local respondio sin contenido utilizable. El asistente conserva las opciones deterministas.",
                "hermes-empty");
        }

        return new(text.Trim(), "hermes-local");
    }

    private sealed record ChatCompletionRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] ChatMessage[] Messages,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_tokens")] int MaxTokens);

    private sealed record ChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record ChatCompletionResponse(
        [property: JsonPropertyName("choices")] ChatChoice[]? Choices);

    private sealed record ChatChoice(
        [property: JsonPropertyName("message")] ChatMessage? Message);
}
