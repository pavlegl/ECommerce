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

        public List<DtoProduct> GetAvailableProducts(string regionAlpha3Code)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    var query = (from p in dbc.Products
                                              join pr in dbc.ProductRegions on p.IdProduct equals pr.IdProduct
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
                    if (!String.IsNullOrEmpty(regionAlpha3Code))
                        query = query.Where(l => l.CountryAlpha3Code == regionAlpha3Code);

                    List<DtoProduct> lsProducts = (List<DtoProduct>)query.ToList();
                    return lsProducts;

                    /*dbc.Products.Join(dbc.ProductRegions, p => p.IdProduct, pr => pr.IdProduct, (product, productRegion) =>
                    new DtoProduct {
                         
                    });
                    FormattableString sQuery = $"select P.*, PT.Name as ProductType, B.Name as BrandName from Product p inner join Brand b on b.IdBrand=p.IdBrand inner join ProductType pr on pr.IdProductType=p.IdProductType where ";
                    List<DtoProduct> lsProducts = null;
                    if (String.IsNullOrEmpty(regionAlpha3Code))
                        lsProducts = dbc.Database.SqlQuery<DtoProduct>(sQuery).Where(l=> l.).ToList();
                    else
                        lsProducts = dbc.Database.SqlQuery<DtoProduct>(sQuery + "lalala").ToList();
                    return ECommerce.Common.Map(Product, ProductDAL, List<Product>, List<ProductDto>)dbc.Products.OrderBy(l=>l.Name).ToList();
                    */

                    /*return ECommerce.Common.Map<Models.User, DtoUser, List<Models.User>, List<DtoUser>>(dbc.Users.ToList());*/
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetProducts(): " + ECommerce.Common.getWholeException(ex));
            }

        }
    }
}