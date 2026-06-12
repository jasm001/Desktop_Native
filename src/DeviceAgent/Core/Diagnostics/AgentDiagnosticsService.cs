using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public sealed class AgentDiagnosticsService(
    IWindowsDiagnosticCollector windowsCollector,
    IStorageDiagnosticCollector storageCollector,
    IMemoryDiagnosticCollector memoryCollector,
    INetworkDiagnosticCollector networkCollector,
    IAgentVersionDiagnosticCollector agentVersionCollector,
    AgentActionAuthorizationPolicy authorizationPolicy,
    TimeProvider timeProvider)
{
    public async Task<AgentDiagnosticCommandResult> CollectAsync(
        GetAgentDiagnosticsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        AgentError? validationError = ValidateRequest(request);
        if (validationError is not null)
        {
            return AgentDiagnosticCommandResult.Rejected(
                validationError.Code,
                validationError.Message);
        }

        AuthorizedAgentAction? action = authorizationPolicy.Find(
            request.ActionId,
            request.TargetId,
            request.TargetVersion);
        if (action is null)
        {
            return AgentDiagnosticCommandResult.Rejected(
                AgentErrorCode.UnauthorizedAction,
                "The requested action and target are not present in the installed development policy.");
        }

        Task<WindowsDiagnosticResult> windowsTask = CollectSafelyAsync(
            windowsCollector.CollectAsync,
            new(
                DiagnosticCollectionStatus.Unavailable,
                "windows.unavailable",
                Version: null,
                Architecture: null),
            cancellationToken);
        Task<StorageDiagnosticResult> storageTask = CollectSafelyAsync(
            storageCollector.CollectAsync,
            new(
                DiagnosticCollectionStatus.Unavailable,
                "storage.unavailable",
                CapacityBytes: null,
                AvailableBytes: null,
                FixedVolumeCount: null),
            cancellationToken);
        Task<MemoryDiagnosticResult> memoryTask = CollectSafelyAsync(
            memoryCollector.CollectAsync,
            new(
                DiagnosticCollectionStatus.Unavailable,
                "memory.unavailable",
                TotalBytes: null,
                AvailableBytes: null),
            cancellationToken);
        Task<NetworkDiagnosticResult> networkTask = CollectSafelyAsync(
            networkCollector.CollectAsync,
            new(
                DiagnosticCollectionStatus.Unavailable,
                "network.unavailable",
                NetworkAvailable: null,
                DomainReachabilityStatus.Unknown),
            cancellationToken);
        Task<AgentVersionDiagnosticResult> agentTask = CollectSafelyAsync(
            agentVersionCollector.CollectAsync,
            new(
                DiagnosticCollectionStatus.Unavailable,
                "agent.version.unavailable",
                Version: null),
            cancellationToken);

        await Task.WhenAll(windowsTask, storageTask, memoryTask, networkTask, agentTask);

        WindowsDiagnosticResult windows = await windowsTask;
        StorageDiagnosticResult storage = await storageTask;
        MemoryDiagnosticResult memory = await memoryTask;
        NetworkDiagnosticResult network = await networkTask;
        AgentVersionDiagnosticResult agent = await agentTask;
        IReadOnlyList<AgentPrerequisiteResult> prerequisites =
            AgentPrerequisiteEvaluator.Evaluate(
                action.Prerequisites,
                windows,
                storage,
                memory,
                network);

        return AgentDiagnosticCommandResult.Available(
            new(
                timeProvider.GetUtcNow(),
                windows,
                storage,
                memory,
                network,
                agent,
                prerequisites));
    }

    private static async Task<TResult> CollectSafelyAsync<TResult>(
        Func<CancellationToken, Task<TResult>> collect,
        TResult unavailableResult,
        CancellationToken cancellationToken)
    {
        try
        {
            return await collect(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return unavailableResult;
        }
    }

    private static AgentError? ValidateRequest(GetAgentDiagnosticsRequest request)
    {
        return ValidateIdentifier(request.ActionId, nameof(request.ActionId))
            ?? ValidateIdentifier(request.TargetId, nameof(request.TargetId))
            ?? ValidateIdentifier(request.TargetVersion, nameof(request.TargetVersion));
    }

    private static AgentError? ValidateIdentifier(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 128)
        {
            return new(
                AgentErrorCode.InvalidMessage,
                $"{fieldName} must contain between 1 and 128 characters.");
        }

        return value.All(character =>
            char.IsAsciiLetterOrDigit(character)
            || character is '.' or '-' or '_' or ':')
            ? null
            : new(
                AgentErrorCode.InvalidMessage,
                $"{fieldName} contains unsupported characters.");
    }
}
