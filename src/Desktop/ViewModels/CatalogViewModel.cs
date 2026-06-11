using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class CatalogViewModel
{
    public IReadOnlyList<SoftwarePreview> Software { get; } =
    [
        new("Visual Studio Code", "Desarrollo", "1.100", "Aprobado", "\uE943"),
        new("7-Zip", "Utilidades", "24.09", "Aprobado", "\uE8B7"),
        new("Git for Windows", "Desarrollo", "2.49", "Aprobado", "\uE756"),
        new("Power BI Desktop", "Datos", "Mayo 2026", "En revisión", "\uE9D2"),
        new("WinSCP", "Transferencia segura", "6.5", "Aprobado", "\uE8C8"),
        new("SQL Server Management Studio", "Datos", "21", "Requiere validación", "\uE968"),
    ];
}
