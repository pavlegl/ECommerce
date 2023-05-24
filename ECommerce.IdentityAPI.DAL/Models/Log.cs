using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class Log
{
    public int Id { get; set; }

    public string Action { get; set; } = null!;

    public int? IdObj { get; set; }

    public string? Description { get; set; }

    public int IdUser { get; set; }

    public DateTime ActionDateTime { get; set; }
}
