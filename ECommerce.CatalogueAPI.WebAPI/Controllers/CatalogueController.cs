#nullable disable
using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace ECommerce.CatalogueAPI.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        IConfiguration _config;
        IECLogger _logger = null;
        IProductBL _productBl = null;
        BaseECExceptionHandler _excHandler = null;

        public CatalogueController(IConfiguration config, IProductBL productBl, BaseECExceptionHandler excHandler, IECLogger logger)
        {
            _config = config;
            _productBl = productBl;
            _logger = logger;
            _excHandler = excHandler;
        }

        /// <summary>
        /// Authenticates the user and returns JWT.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        protected ActionResult AuthenticateUser([FromForm] string username, [FromForm] string password)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        /// <summary>
        /// Returns available active products for the region.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<DtoProduct>> GetAvailableProducts()
        {
            try
            {
                List<DtoProduct> ls = _productBl.GetAvailableProducts();
                foreach (var item in ls)
                {
                    _logger.w2l(item.ProductName, EnumTypeOfLog.Information, null, Request);
                }
                return ls;
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        private void checkAuthorizationRaiseExc()
        {
            Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader);
            if (authorizationHeader.Count == 0) // String.IsNullOrEmpty(authorizationHeader.ToString()))
                throw new ECException(StatusCodes.Status400BadRequest, "Authorization header is empty.");
            if (!authorizationHeader[0].ToLower().StartsWith("bearer"))
                throw new ECException(StatusCodes.Status400BadRequest, "Not a Bearer authentication.");
            string[] arAuth = authorizationHeader[0].Split(' ');
            string jwt = arAuth[1];


        }

        private async Task callECommerceIdentityApi()
        {
            using (var httpClient = new HttpClient())
            {
                string sUrlIdentityApi = _config["UrlECommerceIdentityApi"];
                SvcIdentityApi svc = new SvcIdentityApi(sUrlIdentityApi, httpClient);
                //......
                //......
                //......
            }
        }



    }
}