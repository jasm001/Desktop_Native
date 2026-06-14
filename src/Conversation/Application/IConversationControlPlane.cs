using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.Conversation.Application;

public interface IConversationControlPlane
{
    Task<CreateSoftwareInstallationData?> CreateSoftwareInstallationAsync(
        string correlationId,
        string idempotencyKey,
        string deviceId,
        string productId,
        string productVersion,
        CancellationToken cancellationToken);

    Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
        string correlationId,
        string requestId,
        CancellationToken cancellationToken);

    Task<ControlPlaneBotCase?> GetBotCaseAsync(
        string correlationId,
        string requestId,
        CancellationToken cancellationToken);
}
