namespace ITSupportNative.Desktop.ControlPlane;

public sealed record ControlPlaneClientOptions(Uri BaseAddress)
{
    public static ControlPlaneClientOptions? FromEnvironment()
    {
        bool enabled =
            string.Equals(
                Environment.GetEnvironmentVariable("IT_SUPPORT_ENVIRONMENT"),
                "development",
                StringComparison.Ordinal)
            && string.Equals(
                Environment.GetEnvironmentVariable("LOCAL_CONTROL_PLANE_ENABLED"),
                "true",
                StringComparison.Ordinal);
        string? value =
            Environment.GetEnvironmentVariable("IT_SUPPORT_CONTROL_PLANE_URL");

        if (!enabled || !Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
        {
            return null;
        }

        if (!uri.IsLoopback || uri.Scheme is not ("http" or "https"))
        {
            return null;
        }

        return new(uri);
    }
}
