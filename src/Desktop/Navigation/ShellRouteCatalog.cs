using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.Navigation;

public static class ShellRouteCatalog
{
    public const string Home = "home";
    public const string Catalog = "catalog";
    public const string Assistant = "assistant";
    public const string Requests = "requests";
    public const string DeviceHealth = "device-health";

    public static IReadOnlyList<ShellRoute> All { get; } =
    [
        new(Home, "Inicio", "\uE80F"),
        new(Catalog, "Catálogo", "\uE719"),
        new(Assistant, "Asistente", "\uE8BD"),
        new(Requests, "Solicitudes", "\uE8A5"),
        new(DeviceHealth, "Salud del equipo", "\uE9D9"),
    ];
}
