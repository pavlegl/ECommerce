namespace ECommerce.CatalogueAPI.Common
{
    public interface IProductDAL
    {
        List<DtoProduct> GetAvailableProducts(string regionAlpha3Code);
        List<DtoProduct> GetAvailableProductsByProductType(string regionAlpha3Code, int idProductType);
        List<DtoProduct> GetAvailableProductByIdProduct(string regionAlpha3Code, int idProduct);
    }
}
