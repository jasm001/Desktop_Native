namespace ITSupportNative.DeviceAgent.Jobs;

public interface IAgentJobStore
{
    Task<IReadOnlyList<AgentJobRecord>> LoadAsync(CancellationToken cancellationToken);

    Task SaveAsync(
        IReadOnlyList<AgentJobRecord> jobs,
        CancellationToken cancellationToken);
}
