using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Configuration;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed class WindowsDomainReachabilityProbe(IOptions<DeviceAgentOptions> options)
    : IDomainReachabilityProbe
{
    private const int MinimumTimeoutMilliseconds = 100;
    private const int MaximumTimeoutMilliseconds = 10000;

    public async Task<DomainReachabilityStatus> ProbeAsync(
        CancellationToken cancellationToken)
    {
        string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
        if (string.IsNullOrWhiteSpace(domainName))
        {
            return DomainReachabilityStatus.NotApplicable;
        }

        int timeoutMilliseconds = Math.Clamp(
            options.Value.DomainProbeTimeoutMilliseconds,
            MinimumTimeoutMilliseconds,
            MaximumTimeoutMilliseconds);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(timeoutMilliseconds);

        try
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(
                domainName,
                timeout.Token);
            return addresses.Length > 0
                ? DomainReachabilityStatus.Reachable
                : DomainReachabilityStatus.Unreachable;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return DomainReachabilityStatus.TimedOut;
        }
        catch (SocketException)
        {
            return DomainReachabilityStatus.Unreachable;
        }
    }
}
