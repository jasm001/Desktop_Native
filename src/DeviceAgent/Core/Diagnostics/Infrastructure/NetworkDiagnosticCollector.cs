using System.Net.NetworkInformation;
using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed class NetworkDiagnosticCollector(IDomainReachabilityProbe domainProbe)
    : INetworkDiagnosticCollector
{
    public async Task<NetworkDiagnosticResult> CollectAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool networkAvailable = NetworkInterface.GetIsNetworkAvailable();
        if (!networkAvailable)
        {
            return new(
                DiagnosticCollectionStatus.Available,
                "network.offline",
                NetworkAvailable: false,
                DomainReachabilityStatus.Unreachable);
        }

        DomainReachabilityStatus domainReachability =
            await domainProbe.ProbeAsync(cancellationToken);
        return new(
            domainReachability == DomainReachabilityStatus.TimedOut
                ? DiagnosticCollectionStatus.TimedOut
                : DiagnosticCollectionStatus.Available,
            domainReachability switch
            {
                DomainReachabilityStatus.Reachable => "network.domain_reachable",
                DomainReachabilityStatus.Unreachable => "network.domain_unreachable",
                DomainReachabilityStatus.TimedOut => "network.domain_timeout",
                DomainReachabilityStatus.NotApplicable => "network.domain_not_applicable",
                _ => "network.domain_unknown",
            },
            NetworkAvailable: true,
            domainReachability);
    }
}
