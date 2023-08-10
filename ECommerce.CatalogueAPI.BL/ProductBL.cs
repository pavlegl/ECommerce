#nullable disable
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL.Models;

namespace ECommerce.CatalogueAPI.BL
{
    public class ProductBL : IProductBL
    {
        IProductDAL _productDal = null;
        string _appRegionAlpha3Code = null;

        public ProductBL(IProductDAL productDal, string appRegionAlpha3Code)
        {
            _productDal = productDal;
            _appRegionAlpha3Code = appRegionAlpha3Code;
        }

        public List<DtoProduct> GetAvailableProducts()
        {
            try
            {
                return _productDal.GetAvailableProducts(_appRegionAlpha3Code);
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
                return _productDal.GetAvailableProductsByProductType(_appRegionAlpha3Code, idProductType);
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
                return _productDal.GetAvailableProductByIdProduct(_appRegionAlpha3Code, idProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProductByIdProduct(): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

    }
}