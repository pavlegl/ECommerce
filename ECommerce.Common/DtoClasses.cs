using System.Data;

namespace ECommerce.Models
{
    public class DtoUserDal
    {
        public int? IdUser { get; set; }
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
    }

    public class DtoRoleDal
    {
        public int IdRole { get; set; }
        public string Name { get; set; } = null!;
    }

    public class DtoUserRoleDal
    {
        public int IdUserRole { get; set; }
        public int IdUser { get; set; }
        public int IdRole { get; set; }
    }
}
