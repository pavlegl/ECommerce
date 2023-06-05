namespace ECommerce.CatalogueAPI.Common
{
    public class DtoProduct
    {
        public int IdProduct { get; set; }
        public int IdProductType { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsCustomizable { get; set; }
        public string? CountryAlpha3Code { get; set; }

        public string? ProductTypeName { get; set; }
        public string? BrandName { get; set; }

    }
}