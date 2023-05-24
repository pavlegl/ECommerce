using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int IdUser { get; set; }

    public int IdCartItem { get; set; }

    public DateTime DateCartCreated { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
