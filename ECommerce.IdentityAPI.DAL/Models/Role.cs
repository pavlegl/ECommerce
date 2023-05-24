using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string Name { get; set; } = null!;

    public int Uchanged { get; set; }

    public DateTime Tchanged { get; set; }

    public virtual User UchangedNavigation { get; set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
