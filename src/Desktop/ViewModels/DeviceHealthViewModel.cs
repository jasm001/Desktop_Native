using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class DeviceHealthViewModel
{
    public IReadOnlyList<StatusMetric> Metrics { get; } =
    [
        new("Windows", "Windows 11", "Versión de demostración 24H2", "\uE782", "Neutral"),
        new("Procesador", "Normal", "Carga sintetizada: 18 %", "\uE950", "Success"),
        new("Memoria", "Correcta", "Carga sintetizada: 42 %", "\uE950", "Success"),
        new("Almacenamiento", "Correcto", "214 GB libres de 674 GB", "\uEDA2", "Success"),
        new("Red", "Conectada", "Adaptador local de demostración", "\uE701", "Success"),
        new("Agente", "No conectado", "Se habilitará en el Bloque 4", "\uE7BA", "Neutral"),
    ];
}
