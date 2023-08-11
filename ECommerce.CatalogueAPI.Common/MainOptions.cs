#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.CatalogueAPI.Common
{
    public class MainOptions
    {
        public const string OptionsName = "MainOptions";
        public string UrlECommerceIdentityApi { get; set; }
        public ConnectionStringsOption ConnectionStrings { get; set; }
        public string CurrentRegionAlpha3Code { get; set; }
    }

    public class ConnectionStringsOption
    {
        public string ECommerceConnString { get; set; }
    }
}
