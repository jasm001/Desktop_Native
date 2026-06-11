using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class AssistantViewModel
{
    public IReadOnlyList<AssistantSuggestion> Suggestions { get; } =
    [
        new("Buscar software", "¿Qué herramientas de desarrollo están disponibles?", "\uE721"),
        new("Revisar mi equipo", "Explícame el estado general de este equipo.", "\uE9D9"),
        new("Consultar una solicitud", "Quiero conocer el estado de una solicitud.", "\uE8A5"),
    ];
}
