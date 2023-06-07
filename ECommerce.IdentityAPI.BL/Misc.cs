using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL;
using ECommerce.IdentityAPI.DAL.Models;
using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.IdentityAPI.BL
{
    /*internal class ClientDal : IDisposable
    {
        private string _sUrlClientDal = null;
        private HttpClient _httpClient = null;
        private swaggerClient _swaggerClient = null;

        public ClientDal(string urlClientDal)
        {
            _sUrlClientDal = urlClientDal;
            HttpClient httpClient = new HttpClient();
            _swaggerClient = new swaggerClient(_sUrlClientDal, httpClient);
        }

        public swaggerClient Swc { get { return _swaggerClient; } }
    }*/

    internal class Misc
    {
        internal static void validateNewUser(DtoUser user, IUserDAL userDal)
        {
            if (user == null)
                throw new ECException("User object is null.");

            string[] arSupportedCountriesCodes = { "AUT", "BEL", "BGR", "HRV", "CYP", "CZE", "DNK", "EST", "FIN", "FRA", "DEU", "GRC", "HUN", "IRL", "ITA", "LVA", "LTU", "LUX", "MLT", "NLD", "POL", "PRT", "ROU", "SVK", "SVN", "ESP", "SWE", "GBR", "MNE" };
            char[] arMandatoryPasswordSpecialChars = { '!', '#', '_', '$' };
            string sRegExUsername = @"^[a-z]+\.[a-z|0-9]+$";
            string sRegExEmailAddress = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            string[] arCountriesWithMandatoryPostCodes = { "GBR", "ESP" };

            if (!arSupportedCountriesCodes.Contains(user.CountryAlpha3Code))
                throw new ECException(StatusCodes.Status400BadRequest, "The country is not supported.");

            if (arCountriesWithMandatoryPostCodes.Contains(user.CountryAlpha3Code) && String.IsNullOrEmpty(user.Postcode))
                throw new ECException(StatusCodes.Status400BadRequest, "User from this country must provide postcode.");

            if (!String.IsNullOrEmpty(user.EmailAddress))
            {
                Regex regEmailAddress = new Regex(sRegExEmailAddress, RegexOptions.IgnoreCase);
                if (!regEmailAddress.IsMatch(user.EmailAddress))
                    throw new ECException(StatusCodes.Status400BadRequest, "Bad format of the email address.");
            }

            List<DtoUser> lsUsers = (List<DtoUser>)userDal.GetUsers();
            if (lsUsers.FirstOrDefault(l => l.EmailAddress != null && l.EmailAddress.ToLower() == user.EmailAddress?.ToLower()) != null)
                throw new ECException(StatusCodes.Status409Conflict, "Provided email address is already used by another user.");

            Regex regUsername = new Regex(sRegExUsername, RegexOptions.IgnoreCase);
            if (!regUsername.IsMatch(user.Username) || user.Username.Length < 3 || user.Username.Length > 50)
                throw new ECException(StatusCodes.Status400BadRequest, "Username doesn't meet format requirements.");

            // --- Password must have at least 8 characters, 1 special character and 1 digit ---
            if (user.Password.Length < 8 || user.Password.Length > 16 || user.Password.IndexOfAny(arMandatoryPasswordSpecialChars) < 0
                    || !user.Password.Any(l => char.IsDigit(l)))
                throw new ECException(StatusCodes.Status400BadRequest, "Password is not properly formatted.");
        }
    }
}
