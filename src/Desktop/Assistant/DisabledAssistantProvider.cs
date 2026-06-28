namespace ITSupportNative.Desktop.Assistant;

public sealed class DisabledAssistantProvider : IAssistantProvider
{
    public bool IsAvailable => false;

    public Task<AssistantProviderReply> GetResponseAsync(
        string message,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            new AssistantProviderReply(
                "Hermes local no esta configurado. El asistente conserva las opciones deterministas.",
                "disabled"));
    }
}
