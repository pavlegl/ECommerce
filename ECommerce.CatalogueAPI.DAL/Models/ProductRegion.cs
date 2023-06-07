using System;
using System.Collections.Generic;

namespace ECommerce.CatalogueAPI.DAL.Models;

public partial class ProductRegion
{
    public int IdProductRegion { get; set; }

    public string CountryAlpha3Code { get; set; } = null!;

    public int IdProduct { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
