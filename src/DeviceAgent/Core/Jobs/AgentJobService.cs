using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentJobService(
    IAgentJobStore store,
    AgentActionAuthorizationPolicy authorizationPolicy,
    TimeProvider timeProvider,
    IAgentActionExecutor actionExecutor) : IDisposable
{
    private const string QueuedEvidenceCode = "job.accepted";
    private const string RunningEvidenceCode = "job.simulation.started";
    private const string RecoveredEvidenceCode = "job.recovered";
    private const string CompletedEvidenceCode = "job.simulation.verified";
    private const string CancelledEvidenceCode = "job.cancelled";
    private const string ExecutionStartedEvidenceCode = "job.execution.started";

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, AgentJobRecord> _jobs = new(StringComparer.Ordinal);
    private bool _initialized;

    public AgentJobService(
        IAgentJobStore store,
        AgentActionAuthorizationPolicy authorizationPolicy,
        TimeProvider timeProvider)
        : this(
            store,
            authorizationPolicy,
            timeProvider,
            new DenyAgentActionExecutor())
    {
    }

    public void Dispose()
    {
        _gate.Dispose();
    }

    public async Task<AgentJobCommandResult> StartAsync(
        StartAgentJobRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        AgentError? validationError = ValidateStartRequest(request);
        if (validationError is not null)
        {
            return AgentJobCommandResult.Rejected(
                validationError.Code,
                validationError.Message);
        }

        if (!authorizationPolicy.IsAuthorized(request))
        {
            return AgentJobCommandResult.Rejected(
                AgentErrorCode.UnauthorizedAction,
                "The requested action and target are not present in the installed development policy.");
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedCoreAsync(cancellationToken);

            AgentJobRecord? existing = _jobs.Values.FirstOrDefault(job =>
                string.Equals(job.IdempotencyKey, request.IdempotencyKey, StringComparison.Ordinal));
            if (existing is not null)
            {
                bool sameRequest = string.Equals(existing.RequestId, request.RequestId, StringComparison.Ordinal)
                    && string.Equals(existing.ActionId, request.ActionId, StringComparison.Ordinal)
                    && string.Equals(existing.TargetId, request.TargetId, StringComparison.Ordinal)
                    && string.Equals(existing.TargetVersion, request.TargetVersion, StringComparison.Ordinal);

                return sameRequest
                    ? AgentJobCommandResult.Accepted(existing.ToSnapshot(), isDuplicate: true)
                    : AgentJobCommandResult.Rejected(
                        AgentErrorCode.IdempotencyConflict,
                        "The idempotency key is already bound to a different request.");
            }

            DateTimeOffset now = timeProvider.GetUtcNow();
            string jobId = $"job-{Guid.NewGuid():N}";
            var evidence = new AgentJobEvidence(
                QueuedEvidenceCode,
                "The job was accepted by the installed development policy.",
                now);
            var job = new AgentJobRecord(
                jobId,
                request.RequestId,
                request.IdempotencyKey,
                request.ActionId,
                request.TargetId,
                request.TargetVersion,
                AgentJobState.Queued,
                ProgressPercent: 0,
                RecoveryCount: 0,
                CreatedAt: now,
                UpdatedAt: now,
                Evidence: [evidence]);

            _jobs.Add(jobId, job);
            await PersistCoreAsync(cancellationToken);
            return AgentJobCommandResult.Accepted(job.ToSnapshot());
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<AgentJobCommandResult> GetAsync(
        string jobId,
        CancellationToken cancellationToken)
    {
        AgentError? validationError = ValidateIdentifier(jobId, nameof(jobId));
        if (validationError is not null)
        {
            return AgentJobCommandResult.Rejected(
                validationError.Code,
                validationError.Message);
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedCoreAsync(cancellationToken);
            return _jobs.TryGetValue(jobId, out AgentJobRecord? job)
                ? AgentJobCommandResult.Accepted(job.ToSnapshot())
                : AgentJobCommandResult.Rejected(
                    AgentErrorCode.JobNotFound,
                    "The requested job does not exist.");
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<AgentJobCommandResult> CancelAsync(
        string jobId,
        CancellationToken cancellationToken)
    {
        AgentError? validationError = ValidateIdentifier(jobId, nameof(jobId));
        if (validationError is not null)
        {
            return AgentJobCommandResult.Rejected(
                validationError.Code,
                validationError.Message);
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedCoreAsync(cancellationToken);
            if (!_jobs.TryGetValue(jobId, out AgentJobRecord? job))
            {
                return AgentJobCommandResult.Rejected(
                    AgentErrorCode.JobNotFound,
                    "The requested job does not exist.");
            }

            if (job.State is not (AgentJobState.Queued or AgentJobState.Running))
            {
                return AgentJobCommandResult.Rejected(
                    AgentErrorCode.JobNotCancellable,
                    "Only queued or safely cancellable running jobs can be cancelled.");
            }

            AuthorizedAgentAction? action = authorizationPolicy.Find(
                job.ActionId,
                job.TargetId,
                job.TargetVersion);
            if (job.State == AgentJobState.Running
                && action?.SupportsRunningCancellation != true)
            {
                return AgentJobCommandResult.Rejected(
                    AgentErrorCode.JobNotCancellable,
                    "The running adapter cannot be interrupted safely.");
            }

            DateTimeOffset now = timeProvider.GetUtcNow();
            AgentJobRecord cancelled = job with
            {
                State = AgentJobState.Cancelled,
                UpdatedAt = now,
                Evidence =
                [
                    .. job.Evidence,
                    new(
                        CancelledEvidenceCode,
                        "The job was cancelled before an unsafe device change.",
                        now),
                ],
            };
            _jobs[jobId] = cancelled;
            await PersistCoreAsync(cancellationToken);
            return AgentJobCommandResult.Accepted(cancelled.ToSnapshot());
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task AdvancePendingJobsAsync(CancellationToken cancellationToken)
    {
        AgentJobRecord? claimedJob = null;
        AuthorizedAgentAction? claimedAction = null;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedCoreAsync(cancellationToken);
            DateTimeOffset now = timeProvider.GetUtcNow();
            bool changed = false;

            foreach ((string jobId, AgentJobRecord job) in _jobs.ToArray())
            {
                AuthorizedAgentAction? action = authorizationPolicy.Find(
                    job.ActionId,
                    job.TargetId,
                    job.TargetVersion);
                if (action is null)
                {
                    continue;
                }

                if (action.ExecutionKind != AgentActionExecutionKind.Simulated)
                {
                    if (claimedJob is null && job.State == AgentJobState.Queued)
                    {
                        claimedAction = action;
                        claimedJob = StartExecution(job, now);
                        _jobs[jobId] = claimedJob;
                        changed = true;
                    }

                    continue;
                }

                AgentJobRecord? advanced = AdvanceSimulation(job, now);
                if (advanced is null)
                {
                    continue;
                }

                _jobs[jobId] = advanced;
                changed = true;
            }

            if (changed)
            {
                await PersistCoreAsync(cancellationToken);
            }
        }
        finally
        {
            _gate.Release();
        }

        if (claimedJob is null || claimedAction is null)
        {
            return;
        }

        AgentActionExecutionResult executionResult;
        if (!actionExecutor.CanExecute(claimedAction))
        {
            executionResult = new(
                Success: false,
                "job.execution.adapter_unavailable",
                "The installed policy did not resolve an execution adapter.");
        }
        else
        {
            try
            {
                executionResult = await actionExecutor.ExecuteAsync(
                    claimedAction,
                    cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception)
            {
                executionResult = new(
                    Success: false,
                    "job.execution.internal_failure",
                    "The adapter failed without exposing internal details.");
            }
        }

        await CompleteExecutionAsync(
            claimedJob.JobId,
            executionResult,
            cancellationToken);
    }

    private async Task EnsureInitializedCoreAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        IReadOnlyList<AgentJobRecord> storedJobs = await store.LoadAsync(cancellationToken);
        DateTimeOffset now = timeProvider.GetUtcNow();
        bool recovered = false;

        foreach (AgentJobRecord storedJob in storedJobs)
        {
            AgentJobRecord job = storedJob;
            if (job.State == AgentJobState.Running)
            {
                job = job with
                {
                    State = AgentJobState.Queued,
                    RecoveryCount = job.RecoveryCount + 1,
                    UpdatedAt = now,
                    Evidence =
                    [
                        .. job.Evidence,
                        new(
                            RecoveredEvidenceCode,
                            "The interrupted job was queued after agent restart.",
                            now),
                    ],
                };
                recovered = true;
            }

            _jobs[job.JobId] = job;
        }

        _initialized = true;
        if (recovered)
        {
            await PersistCoreAsync(cancellationToken);
        }
    }

    private async Task PersistCoreAsync(CancellationToken cancellationToken)
    {
        AgentJobRecord[] jobs = [.. _jobs.Values.OrderBy(job => job.CreatedAt)];
        await store.SaveAsync(jobs, cancellationToken);
    }

    private static AgentJobRecord StartExecution(
        AgentJobRecord job,
        DateTimeOffset now)
    {
        return job with
        {
            State = AgentJobState.Running,
            ProgressPercent = 25,
            UpdatedAt = now,
            Evidence =
            [
                .. job.Evidence,
                new(
                    ExecutionStartedEvidenceCode,
                    "The agent started a fixed allowlisted execution adapter.",
                    now),
            ],
        };
    }

    private async Task CompleteExecutionAsync(
        string jobId,
        AgentActionExecutionResult result,
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedCoreAsync(cancellationToken);
            if (!_jobs.TryGetValue(jobId, out AgentJobRecord? job)
                || job.State != AgentJobState.Running)
            {
                return;
            }

            DateTimeOffset now = timeProvider.GetUtcNow();
            _jobs[jobId] = job with
            {
                State = result.Success
                    ? AgentJobState.Succeeded
                    : AgentJobState.Failed,
                ProgressPercent = 100,
                UpdatedAt = now,
                Evidence =
                [
                    .. job.Evidence,
                    new(result.EvidenceCode, result.Summary, now),
                ],
            };
            await PersistCoreAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static AgentJobRecord? AdvanceSimulation(
        AgentJobRecord job,
        DateTimeOffset now)
    {
        if (job.State == AgentJobState.Queued)
        {
            return job with
            {
                State = AgentJobState.Running,
                ProgressPercent = 25,
                UpdatedAt = now,
                Evidence =
                [
                    .. job.Evidence,
                    new(
                        RunningEvidenceCode,
                        "The agent started a simulated execution without invoking an installer.",
                        now),
                ],
            };
        }

        if (job.State != AgentJobState.Running)
        {
            return null;
        }

        int progress = Math.Min(job.ProgressPercent + 25, 100);
        if (progress < 100)
        {
            return job with
            {
                ProgressPercent = progress,
                UpdatedAt = now,
            };
        }

        return job with
        {
            State = AgentJobState.Succeeded,
            ProgressPercent = 100,
            UpdatedAt = now,
            Evidence =
            [
                .. job.Evidence,
                new(
                    CompletedEvidenceCode,
                    "The simulated verification completed; no device change was performed.",
                    now),
            ],
        };
    }

    private static AgentError? ValidateStartRequest(StartAgentJobRequest request)
    {
        return ValidateIdentifier(request.RequestId, nameof(request.RequestId))
            ?? ValidateIdentifier(request.IdempotencyKey, nameof(request.IdempotencyKey))
            ?? ValidateIdentifier(request.ActionId, nameof(request.ActionId))
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
