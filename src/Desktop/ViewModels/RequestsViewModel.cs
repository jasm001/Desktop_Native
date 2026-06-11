using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class RequestsViewModel
{
    public IReadOnlyList<RequestPreview> Requests { get; } =
    [
        new("REQ-1048", "Evaluación de Power BI Desktop", "En revisión", "Hoy, 10:16", "Datos sintéticos; no representa una solicitud real."),
        new("REQ-1031", "Consulta de compatibilidad de WinSCP", "Resuelta", "Ayer, 14:52", "Se confirmó disponibilidad en el catálogo de demostración."),
        new("REQ-1017", "Orientación sobre almacenamiento", "Cerrada", "8 jun, 11:05", "Consulta informativa sin acciones sobre el equipo."),
    ];
}
