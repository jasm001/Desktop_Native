using ITSupportNative.Desktop.Navigation;

namespace ITSupportNative.WindowsUi.Tests;

public sealed class ShellRouteCatalogTests
{
    [Fact]
    public void DefinesExactlyTheFiveBlockOneDestinations()
    {
        Assert.Collection(
            ShellRouteCatalog.All,
            route => Assert.Equal(ShellRouteCatalog.Home, route.Key),
            route => Assert.Equal(ShellRouteCatalog.Catalog, route.Key),
            route => Assert.Equal(ShellRouteCatalog.Assistant, route.Key),
            route => Assert.Equal(ShellRouteCatalog.Requests, route.Key),
            route => Assert.Equal(ShellRouteCatalog.DeviceHealth, route.Key));
    }

    [Fact]
    public void UsesUniqueKeysAndAccessibleLabels()
    {
        Assert.Equal(
            ShellRouteCatalog.All.Count,
            ShellRouteCatalog.All.Select(route => route.Key).Distinct().Count());
        Assert.All(ShellRouteCatalog.All, route => Assert.False(string.IsNullOrWhiteSpace(route.Label)));
    }
}
