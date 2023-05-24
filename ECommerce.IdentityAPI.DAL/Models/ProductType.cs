using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class ProductType
{
    public int IdProductType { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public int Uchanged { get; set; }

    public int Tchanged { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User UchangedNavigation { get; set; } = null!;
}
