#nullable disable
using ECommerce.CatalogueAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.IdentityAPI.Common
{
    public class MainOptions
    {
        public const string OptionsName = "MainOptions";
        public JwtSettingsSection JwtSettings { get; set; }
        public ConnectionStringsSection ConnectionStrings { get; set; }
    }

    public class ConnectionStringsSection
    {
        public string ECommerceConnString { get; set; }
    }

    public class JwtSettingsSection
    {
        public string SecretKey { get; set; }
    }

}
