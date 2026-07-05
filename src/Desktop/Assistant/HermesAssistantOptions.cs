namespace ITSupportNative.Desktop.Assistant;

public sealed record HermesAssistantOptions(
    Uri BaseEndpoint,
    string ApiKey,
    string Model,
    TimeSpan Timeout)
{
    public const string EnabledVariable = "IT_SUPPORT_HERMES_CHAT_ENABLED";
    public const string BaseEndpointVariable = "IT_SUPPORT_HERMES_BASE_URL";
    public const string ApiKeyVariable = "IT_SUPPORT_HERMES_API_KEY";
    public const string ModelVariable = "IT_SUPPORT_HERMES_MODEL";

    private const string DefaultBaseEndpoint = "http://127.0.0.1:8765/v1";
    private const string DefaultModel = "it-support";

    public static HermesAssistantOptions? FromEnvironment(
        IReadOnlyDictionary<string, string?>? environment = null)
    {
        environment ??= Environment.GetEnvironmentVariables()
            .Cast<System.Collections.DictionaryEntry>()
            .ToDictionary(
                entry => (string)entry.Key,
                entry => entry.Value?.ToString(),
                StringComparer.Ordinal);

        if (!string.Equals(
            Get(environment, EnabledVariable),
            "true",
            StringComparison.Ordinal))
        {
            return null;
        }

        string? apiKey = Get(environment, ApiKeyVariable);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        string baseEndpoint =
            Get(environment, BaseEndpointVariable) ?? DefaultBaseEndpoint;
        if (!Uri.TryCreate(baseEndpoint, UriKind.Absolute, out Uri? endpoint)
            || !IsLoopbackHttp(endpoint))
        {
            return null;
        }

        string model = Get(environment, ModelVariable) ?? DefaultModel;
        if (string.IsNullOrWhiteSpace(model))
        {
            return null;
        }

        return new(
            NormalizeEndpoint(endpoint),
            apiKey,
            model.Trim(),
            TimeSpan.FromSeconds(120));
    }

    private static string? Get(
        IReadOnlyDictionary<string, string?> environment,
        string name)
    {
        return environment.TryGetValue(name, out string? value)
            ? value
            : null;
    }

    private static bool IsLoopbackHttp(Uri endpoint)
    {
        return endpoint.IsLoopback
            && endpoint.Scheme is "http" or "https";
    }

    private static Uri NormalizeEndpoint(Uri endpoint)
    {
        string value = endpoint.AbsoluteUri.TrimEnd('/');
        return new Uri($"{value}/", UriKind.Absolute);
    }
}
