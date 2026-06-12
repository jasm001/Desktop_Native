using ITSupportNative.Catalog.Application;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.Conversation.Application;

public sealed record ConversationTurn(
    ConversationSession Session,
    ConversationResultCode Code,
    CatalogDecision? CatalogDecision,
    bool TransitionApplied,
    bool IsDuplicate);
