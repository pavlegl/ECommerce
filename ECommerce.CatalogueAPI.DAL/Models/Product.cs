using System;
using System.Collections.Generic;

namespace ECommerce.CatalogueAPI.DAL.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public int IdProductType { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal Quantity { get; set; }

    public bool IsActive { get; set; }

    public bool IsCustomizable { get; set; }

    public int Uchanged { get; set; }

    public DateTime Tchanged { get; set; }

    public int? IdBrand { get; set; }

    public virtual Brand? IdBrandNavigation { get; set; }

    public virtual ProductType IdProductTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductRegion> ProductRegions { get; set; } = new List<ProductRegion>();
}
