using ECommerce.CatalogueAPI.Common;

namespace ECommerce.CatalogueAPI.BL
{
    public class ProductBL
    {
        IProductDAL _productDal = null;

        public ProductBL(IProductDAL productDal)
        {
            _productDal = productDal;
        }

        public List<DtoProduct> GetAvailableProducts(string alpha3Code)
        {
            try
            {
                return _productDal.GetAvailableProducts(alpha3Code);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProducts('" + alpha3Code + "'): " + ECommerce.Common.getWholeException(ex));
            }
        }

    }
}