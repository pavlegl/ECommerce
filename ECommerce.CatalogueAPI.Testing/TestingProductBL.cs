using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL;

namespace ECommerce.CatalogueAPI.Testing
{
    public class TestingProductBL
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetAvailableProducts()
        {
            // ----- Arrange -----
            ProductBL productBl = new ProductBL(new ProductDAL(new DAL.Models.EcommerceContext()), "GER");
            // ----- Act -----
            List<DtoProduct> lsDtoProducts = productBl.GetAvailableProducts();
            // ----- Assert -----
            Assert.That(lsDtoProducts.Count, Is.GreaterThan(0));
        }
    }
}