using System.Data;

namespace ECommerce.Models
{

    public class DtoProductDal
    {
        public int IdProduct { get; set; }
        public int IdProductType { get; set; }

        public string ProductName { get; set; } = null!;
        public string ProductTypeName { get; set; } = null!;
        public string BrandName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsCustomizable { get; set; }
    }
}
