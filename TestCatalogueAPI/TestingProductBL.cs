using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using ECommerce.CatalogueAPI.DAL;

namespace TestCatalogueAPI
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetAvailableProducts()
        {
            ProductBL productBl = new ProductBL(new ProductDAL());

            List<DtoProduct> lsDtoProducts = productBl.GetAvailableProducts(null);

            Assert.That(lsDtoProducts.Count, Is.EqualTo(28));
        }
    }
}