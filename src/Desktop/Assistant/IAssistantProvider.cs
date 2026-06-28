namespace ITSupportNative.Desktop.Assistant;

public interface IAssistantProvider
{
    bool IsAvailable { get; }

    Task<AssistantProviderReply> GetResponseAsync(
        string message,
        CancellationToken cancellationToken);
}
