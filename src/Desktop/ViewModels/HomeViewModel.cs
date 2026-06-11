using ITSupportNative.Desktop.Models;
using ITSupportNative.Desktop.Navigation;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class HomeViewModel
{
    public IReadOnlyList<QuickAction> QuickActions { get; } =
    [
        new("Explorar catálogo", "Consulta software disponible para este equipo.", "\uE719", ShellRouteCatalog.Catalog),
        new("Preguntar al asistente", "Recibe orientación sin ejecutar cambios.", "\uE8BD", ShellRouteCatalog.Assistant),
        new("Revisar solicitudes", "Consulta el estado de solicitudes de demostración.", "\uE8A5", ShellRouteCatalog.Requests),
        new("Ver salud del equipo", "Revisa señales locales sintetizadas.", "\uE9D9", ShellRouteCatalog.DeviceHealth),
    ];

    public IReadOnlyList<StatusMetric> HealthMetrics { get; } =
    [
        new("Estado general", "Correcto", "Sin alertas críticas", "\uE73E", "Success"),
        new("Almacenamiento", "68 % libre", "214 GB disponibles", "\uEDA2", "Neutral"),
        new("Memoria", "42 % en uso", "9.3 GB disponibles", "\uE950", "Neutral"),
        new("Conectividad", "En línea", "Red local de demostración", "\uE701", "Success"),
    ];

    public IReadOnlyList<SoftwarePreview> FeaturedSoftware { get; } =
    [
        new("Visual Studio Code", "Desarrollo", "1.100", "Disponible", "\uE943"),
        new("7-Zip", "Utilidades", "24.09", "Disponible", "\uE8B7"),
        new("Power BI Desktop", "Datos", "Mayo 2026", "Revisión", "\uE9D2"),
    ];

    public IReadOnlyList<ActivityPreview> RecentActivity { get; } =
    [
        new("Diagnóstico local consultado", "Solo lectura; no se ejecutaron cambios.", "Hoy, 09:42", "\uE9D9"),
        new("Catálogo sincronizado", "Se cargaron datos sintéticos del entorno local.", "Ayer, 16:18", "\uE895"),
        new("Sesión iniciada", "Identidad local de desarrollo.", "Ayer, 08:05", "\uE77B"),
    ];
}
