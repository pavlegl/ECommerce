using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ECommerce
{
    public class ECException : Exception
    {
        private ObjectResult _httpResponseCode = new OkObjectResult(null);

        public ObjectResult HttpResponseCode { get { return _httpResponseCode; } }

        public ECException(string message) : base(message) { }

        public ECException(ObjectResult httpResponseCode)
        {
            _httpResponseCode = httpResponseCode;
        }
    }
}