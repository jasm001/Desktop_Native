namespace ITSupportNative.Conversation.Domain;

public sealed record ConversationCommand
{
    public ConversationCommand(
        string id,
        ConversationIntent intent,
        string? productReference = null)
    {
        Id = ConversationValue.NormalizeRequired(id, nameof(id));
        Intent = intent;
        ProductReference = ConversationValue.NormalizeOptional(productReference);
    }

    public string Id { get; }

    public ConversationIntent Intent { get; }

    public string? ProductReference { get; }
}
