namespace ITSupportNative.DeviceAgent.Configuration;

public static class DeviceAgentConfigurationPolicy
{
    private const string DisabledProfile = "disabled";
    private const string LocalDemoProfile = "local-demo";
    private const string DevelopmentEnvironment = "Development";

    public static bool IsValid(
        DeviceAgentOptions options,
        string environmentName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);

        bool isDisabled = string.Equals(
            options.ExecutionProfile,
            DisabledProfile,
            StringComparison.Ordinal);
        bool isLocalDemo = string.Equals(
            options.ExecutionProfile,
            LocalDemoProfile,
            StringComparison.Ordinal);
        if (!isDisabled && !isLocalDemo)
        {
            return false;
        }

        if (isLocalDemo
            && !string.Equals(
                environmentName,
                DevelopmentEnvironment,
                StringComparison.Ordinal))
        {
            return false;
        }

        if ((options.JobExecutionEnabled || options.ControlPlaneSyncEnabled)
            && !isLocalDemo)
        {
            return false;
        }

        return true;
    }
}
