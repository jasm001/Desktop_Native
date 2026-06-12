using System.Collections.ObjectModel;

namespace ITSupportNative.Conversation.Domain;

public sealed class ConversationSession
{
    private ConversationSession(
        string id,
        ConversationState state,
        PendingConversationRequest? pendingRequest,
        SyntheticRequest? request,
        IReadOnlyList<string> processedCommandIds)
    {
        Id = id;
        State = state;
        PendingRequest = pendingRequest;
        Request = request;
        ProcessedCommandIds = processedCommandIds;
    }

    public string Id { get; }

    public ConversationState State { get; }

    public PendingConversationRequest? PendingRequest { get; }

    public SyntheticRequest? Request { get; }

    public IReadOnlyList<string> ProcessedCommandIds { get; }

    public static ConversationSession Start(string id)
    {
        return new(
            ConversationValue.NormalizeRequired(id, nameof(id)),
            ConversationState.Query,
            pendingRequest: null,
            request: null,
            new ReadOnlyCollection<string>([]));
    }

    public bool HasProcessed(string commandId)
    {
        return ProcessedCommandIds.Contains(commandId, StringComparer.Ordinal);
    }

    internal ConversationSession Advance(
        string commandId,
        ConversationState state,
        PendingConversationRequest? pendingRequest,
        SyntheticRequest? request)
    {
        string[] processedIds = [.. ProcessedCommandIds, commandId];

        return new(
            Id,
            state,
            pendingRequest,
            request,
            new ReadOnlyCollection<string>(processedIds));
    }
}
