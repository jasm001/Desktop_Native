using System.Collections.ObjectModel;
using ITSupportNative.Catalog.Domain;

namespace ITSupportNative.Catalog.Application;

public sealed class CatalogSearchService
{
    private readonly IReadOnlyList<SoftwareProduct> _products;

    public CatalogSearchService(IReadOnlyList<SoftwareProduct> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products = new ReadOnlyCollection<SoftwareProduct>(products.ToArray());
    }

    public IReadOnlyList<SoftwareProduct> Search(CatalogSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        string searchText = CatalogText.Normalize(criteria.SearchText);

        SoftwareProduct[] matches = _products
            .Where(product => criteria.Status is null || product.Status == criteria.Status)
            .Where(product => criteria.LicenseType is null || product.License.Type == criteria.LicenseType)
            .Where(product => MatchesText(product, searchText))
            .OrderBy(product => product.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(product => product.Id, StringComparer.Ordinal)
            .ToArray();

        return new ReadOnlyCollection<SoftwareProduct>(matches);
    }

    private static bool MatchesText(SoftwareProduct product, string searchText)
    {
        if (searchText.Length == 0)
        {
            return true;
        }

        return SearchableValues(product)
            .Select(CatalogText.Normalize)
            .Any(value => value.Contains(searchText, StringComparison.Ordinal));
    }

    private static IEnumerable<string> SearchableValues(SoftwareProduct product)
    {
        yield return product.Name;
        yield return product.Publisher;
        yield return product.Category;
        yield return product.Version.DisplayName;

        foreach (string alias in product.Aliases)
        {
            yield return alias;
        }
    }
}
