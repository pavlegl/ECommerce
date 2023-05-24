using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ECommerce
{
    public class Common
    {
        public static ObjectResult HandleException(Exception ex)
        {
            if (ex == null)
                return new BadRequestObjectResult(null);
            if (ex is ECException)
                return ((ECException)ex).HttpResponseCode;

            return new BadRequestObjectResult(getWholeException(ex));
        }

        internal static string getWholeException(Exception exc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(exc.Message);
            while (exc.InnerException != null)
            {
                sb.Append(" - " + exc.InnerException.Message);
                exc = exc.InnerException;
            }
            return sb.ToString();
        }

    }
}