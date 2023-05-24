using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class CartItem
{
    public int IdCartItem { get; set; }

    public int IdProduct { get; set; }

    public decimal Quantity { get; set; }

    public string? CustomRequest { get; set; }

    public DateTime DateItemAdded { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
