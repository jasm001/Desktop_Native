using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public interface IDomainReachabilityProbe
{
    Task<DomainReachabilityStatus> ProbeAsync(CancellationToken cancellationToken);
}
