using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class UserRole
{
    public int IdUserRole { get; set; }

    public int IdUser { get; set; }

    public int IdRole { get; set; }

    public int Ucreated { get; set; }

    public DateTime Tcreated { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual User UcreatedNavigation { get; set; } = null!;
}
