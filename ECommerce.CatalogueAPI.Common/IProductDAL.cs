namespace ECommerce.CatalogueAPI.Common
{
    public interface IProductDAL
    {
        List<DtoProduct> GetProducts();
    }
}
