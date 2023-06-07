using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography;

namespace ECommerce.CatalogueAPI.DAL
{
    public class ProductDAL : IProductDAL
    {
        public ProductDAL()
        {
        }

        private IEnumerable<DtoProduct> getAllProductsByRegionQuery(EcommerceContext dbc, string regionAlpha3Code)
        {
            return (from p in dbc.Products
                    join pr in dbc.ProductRegions on new { IdProduct = p.IdProduct, RegionAlpha3Code = regionAlpha3Code } equals new { IdProduct = pr.IdProduct, RegionAlpha3Code = pr.CountryAlpha3Code }
                    join pt in dbc.ProductTypes on p.IdProductType equals pt.IdProductType
                    join b in dbc.Brands on p.IdBrand equals b.IdBrand
                    where p.IsActive == true
                    select new DtoProduct
                    {
                        IdProduct = p.IdProduct,
                        IdProductType = p.IdProductType,
                        ProductName = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        IsActive = p.IsActive,
                        IsCustomizable = p.IsCustomizable,
                        CountryAlpha3Code = pr.CountryAlpha3Code,
                        ProductTypeName = pt.Name,
                        BrandName = b.Name
                    });
        }

        /// <summary>
        /// Returns all active products that are available in the selected region.
        /// </summary>
        /// <param name="regionAlpha3Code">Alpha3Code of the selected region.</param>
        /// <returns>List of products.</returns>
        /// <exception cref="Exception"></exception>
        public List<DtoProduct> GetAvailableProducts(string regionAlpha3Code)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    var query = getAllProductsByRegionQuery(dbc, regionAlpha3Code);
                    /*if (!String.IsNullOrEmpty(regionAlpha3Code))
                        query = query.Where(l => l.CountryAlpha3Code == regionAlpha3Code);*/

                    List<DtoProduct> lsProducts = (List<DtoProduct>)query.ToList();
                    return lsProducts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProducts('" + regionAlpha3Code + "'): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns all active products byte product type that are available in the selected region.
        /// </summary>
        /// <param name="idProductType">Type of product.</param>
        /// <param name="regionAlpha3Code">Alpha3Code of the selected region.</param>
        /// <returns>List of products.</returns>
        /// <exception cref="Exception"></exception>
        public List<DtoProduct> GetAvailableProductsByProductType(string regionAlpha3Code, int idProductType)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    var query = getAllProductsByRegionQuery(dbc, regionAlpha3Code);
                    if (!String.IsNullOrEmpty(regionAlpha3Code))
                        query = query.Where(l => l.CountryAlpha3Code == regionAlpha3Code);

                    query = query.Where(l => l.IdProductType == idProductType);

                    List<DtoProduct> lsProducts = (List<DtoProduct>)query.ToList();
                    return lsProducts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProductsByProductType('" + regionAlpha3Code + "'," + idProductType + "): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns all active products byte product type that are available in the selected region.
        /// </summary>
        /// <param name="idProductType">Type of product.</param>
        /// <param name="regionAlpha3Code">Alpha3Code of the selected region.</param>
        /// <returns>List of products.</returns>
        /// <exception cref="Exception"></exception>
        public List<DtoProduct> GetAvailableProductByIdProduct(string regionAlpha3Code, int idProduct)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    var query = getAllProductsByRegionQuery(dbc, regionAlpha3Code);
                    if (!String.IsNullOrEmpty(regionAlpha3Code))
                        query = query.Where(l => l.CountryAlpha3Code == regionAlpha3Code);

                    query = query.Where(l => l.IdProduct == idProduct);

                    List<DtoProduct> lsProducts = (List<DtoProduct>)query.ToList();
                    return lsProducts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAvailableProductsByIdProduct('" + regionAlpha3Code + "'," + idProduct + "): " + ECommerce.EcCommon.getWholeException(ex));
            }
        }
    }
}