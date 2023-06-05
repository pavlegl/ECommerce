using System;
using System.Collections.Generic;

namespace ECommerce.CatalogueAPI.DAL.Models;

public partial class Brand
{
    public int IdBrand { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public int Uchanged { get; set; }

    public DateTime Tchanged { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
