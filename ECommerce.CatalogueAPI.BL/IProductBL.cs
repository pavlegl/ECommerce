namespace ECommerce.CatalogueAPI.Common
{
    public interface IProductBL
    {
        List<DtoProduct> GetAvailableProducts();
        List<DtoProduct> GetAvailableProductsByProductType(int idProductType);
        List<DtoProduct> GetAvailableProductByIdProduct(int idProduct);
    }
}
