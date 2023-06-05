using ECommerce.CatalogueAPI.Common;
using ECommerce.Models;

namespace ECommerce
{
    public interface IProductDAL
    {
        List<DtoProduct> GetAvailableProducts(string regionAlpha3Code);
    }
}
