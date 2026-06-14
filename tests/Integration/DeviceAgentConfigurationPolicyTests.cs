using ITSupportNative.DeviceAgent.Configuration;

namespace ITSupportNative.IntegrationTests;

public sealed class DeviceAgentConfigurationPolicyTests
{
    [Fact]
    public void DefaultConfigurationIsValidAndDisabled()
    {
        var options = new DeviceAgentOptions();

        bool valid = DeviceAgentConfigurationPolicy.IsValid(
            options,
            "Production");

        Assert.True(valid);
        Assert.Equal("disabled", options.ExecutionProfile);
        Assert.False(options.JobExecutionEnabled);
        Assert.False(options.ControlPlaneSyncEnabled);
    }

    [Fact]
    public void LocalDemoIsAcceptedOnlyInDevelopment()
    {
        var options = new DeviceAgentOptions
        {
            ExecutionProfile = "local-demo",
            JobExecutionEnabled = true,
            ControlPlaneSyncEnabled = true,
        };

        Assert.True(
            DeviceAgentConfigurationPolicy.IsValid(options, "Development"));
        Assert.False(
            DeviceAgentConfigurationPolicy.IsValid(options, "Production"));
    }

    [Fact]
    public void UnknownProfileFailsClosed()
    {
        var options = new DeviceAgentOptions
        {
            ExecutionProfile = "pilot",
        };

        Assert.False(
            DeviceAgentConfigurationPolicy.IsValid(options, "Development"));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void DisabledProfileRejectsEnabledCapabilities(
        bool jobExecutionEnabled,
        bool controlPlaneSyncEnabled)
    {
        var options = new DeviceAgentOptions
        {
            ExecutionProfile = "disabled",
            JobExecutionEnabled = jobExecutionEnabled,
            ControlPlaneSyncEnabled = controlPlaneSyncEnabled,
        };

        Assert.False(
            DeviceAgentConfigurationPolicy.IsValid(options, "Development"));
    }
}
