namespace ITSupportNative.Contracts.ControlPlane;

public sealed record ControlPlaneApiMeta(
    string ApiVersion,
    string CorrelationId);

public sealed record ControlPlaneApiEnvelope<TData>(
    TData Data,
    ControlPlaneApiMeta Meta);

public sealed record CreateSoftwareInstallationRequest(
    bool Confirmed,
    string DeviceId,
    string ProductId,
    string ProductVersion,
    string ActionId);

public sealed record CreateSoftwareInstallationData(
    ControlPlaneSupportRequest Request,
    bool Replayed);

public sealed record GetSupportRequestData(
    ControlPlaneSupportRequest Request);

public sealed record ControlPlaneSupportRequest(
    string Id,
    string Reference,
    string CorrelationId,
    string Status,
    string DeviceId,
    string ProductId,
    string ProductVersion,
    string ActionId,
    DateTimeOffset CreatedAt,
    ControlPlaneExecutionJob Job);

public sealed record ControlPlaneExecutionJob(
    string Id,
    string Status,
    IReadOnlyList<ControlPlaneExecutionEvidence> Evidence);

public sealed record ControlPlaneExecutionEvidence(
    string Code,
    string Summary,
    DateTimeOffset RecordedAt);

public sealed record ClaimAgentJobRequest(string DeviceId);

public sealed record ClaimAgentJobData(ClaimedAgentJob? Job);

public sealed record ClaimedAgentJob(
    string JobId,
    string RequestId,
    string IdempotencyKey,
    string ActionId,
    string TargetId,
    string TargetVersion,
    string ClaimToken,
    DateTimeOffset LeaseExpiresAt);

public sealed record ReportAgentJobResultRequest(
    string ClaimToken,
    string Result,
    IReadOnlyList<ReportAgentEvidence> Evidence);

public sealed record ReportAgentEvidence(
    string Code,
    DateTimeOffset RecordedAt);

public sealed record ReportAgentJobResultData(
    ControlPlaneSupportRequest Request,
    bool Replayed);

public sealed record GetBotCaseData(
    ControlPlaneBotCase Case);

public sealed record ControlPlaneBotCase(
    string Id,
    string RequestId,
    string CorrelationId,
    string Category,
    string Status,
    string Result,
    DateTimeOffset? WaitingForUserSince,
    DateTimeOffset? EscalatedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    ControlPlaneExternalTicket? ExternalTicket);

public sealed record ControlPlaneExternalTicket(
    string Id,
    string Provider,
    string Reference,
    string Category,
    string Status,
    string CorrelationId,
    string ReasonCode,
    string Description,
    DateTimeOffset CreatedAt);
