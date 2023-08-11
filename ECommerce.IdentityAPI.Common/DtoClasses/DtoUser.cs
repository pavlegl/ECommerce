namespace ECommerce.IdentityAPI.Common
{
    public class DtoUser
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
    }

}