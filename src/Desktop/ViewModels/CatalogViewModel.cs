using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Domain;
using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class CatalogViewModel(CatalogSearchService catalogSearch)
{
    public IReadOnlyList<SoftwarePreview> Software { get; } = catalogSearch
        .Search(new CatalogSearchCriteria())
        .Select(ToPreview)
        .ToArray();

    public string ProductCountLabel => $"{Software.Count} productos sintéticos";

    private static SoftwarePreview ToPreview(SoftwareProduct product)
    {
        return new(
            product.Name,
            product.Category,
            product.Version.DisplayName,
            ToStatusLabel(product),
            ToGlyph(product.Category));
    }

    private static string ToStatusLabel(SoftwareProduct product)
    {
        if (product.Status == SoftwareProductStatus.Approved
            && product.License.Type == SoftwareLicenseType.Commercial)
        {
            return "Comercial";
        }

        return product.Status switch
        {
            SoftwareProductStatus.Approved => "Aprobado",
            SoftwareProductStatus.Unlisted => "No listado",
            SoftwareProductStatus.EndOfLife => "EOL",
            SoftwareProductStatus.Prohibited => "Prohibido",
            _ => throw new InvalidOperationException("Estado de catálogo no soportado."),
        };
    }

    private static string ToGlyph(string category)
    {
        return category switch
        {
            "Development" => "\uE943",
            "Data" => "\uE9D2",
            "File transfer" => "\uE8C8",
            "Design" => "\uE790",
            _ => "\uE8B7",
        };
    }
}
