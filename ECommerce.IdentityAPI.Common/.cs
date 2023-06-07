namespace ECommerce.IdentityAPI.Common
{
    public class Misc
    {
        public const int IdRole_Admin = 2;
        public const int IdRole_Customer = 3;
        public class CustomClaims
        {
            public static string IdUser = "IdUser";
            public static string Roles = "Roles";
            public static string IsAdmin = "IsAdmin";
            public static string IsCustomer = "IsCustomer";
        }
    }
}
