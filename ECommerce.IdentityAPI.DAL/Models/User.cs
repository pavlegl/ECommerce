using System;
using System.Collections.Generic;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public bool IsOnHold { get; set; }

    public int Uchanged { get; set; }

    public DateTime Tchanged { get; set; }

    public string Address { get; set; } = null!;

    public string? Postcode { get; set; }

    public string CountryAlpha3Code { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ProductType> ProductTypes { get; set; } = new List<ProductType>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<UserRole> UserRoleIdUserNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUcreatedNavigations { get; set; } = new List<UserRole>();
}
