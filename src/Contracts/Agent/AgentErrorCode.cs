namespace ITSupportNative.Contracts.Agent;

public enum AgentErrorCode
{
    InvalidMessage,
    UnsupportedVersion,
    UnknownMessage,
    UnauthorizedAction,
    IdempotencyConflict,
    JobNotFound,
    JobNotCancellable,
    InternalError,
}
