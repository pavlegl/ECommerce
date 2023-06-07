#nullable disable
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL.Models;

namespace ECommerce.CatalogueAPI.BL
{
    public class ProductBL : IProductBL
    {
        IProductDAL _productDal = null;
        string _regionAlpha3Code = null;

        public ProductBL(IProductDAL productDal)
        {
            _productDal = productDal;
            _regionAlpha3Code = "GER";
        }

        public List<DtoProduct> GetAvailableProducts()
        {
            try
            {
                return _productDal.GetAvailableProducts(_regionAlpha3Code);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProducts(): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

        public List<DtoProduct> GetAvailableProducts(string regionAlpha3Code)
        {
            throw new NotImplementedException();
        }

        public List<DtoProduct> GetAvailableProductsByProductType(int idProductType)
        {
            try
            {
                return _productDal.GetAvailableProductsByProductType(_regionAlpha3Code, idProductType);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProducts(): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

        public List<DtoProduct> GetAvailableProductByIdProduct(int idProduct)
        {
            try
            {
                return _productDal.GetAvailableProductByIdProduct(_regionAlpha3Code, idProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProductByIdProduct(): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

    }
}