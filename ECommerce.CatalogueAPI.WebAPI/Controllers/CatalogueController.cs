#nullable disable
using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ECommerce.CatalogueAPI.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        IECConfig _ecConfig = null;
        IProductBL _productBl = null;

        public CatalogueController(IECConfig ecConfig, IProductBL productBl)
        {
            _ecConfig = ecConfig;
            if (_ecConfig == null)
                _ecConfig = new ECConfig(new ECExceptionHttpResponseHandler(null), new ECLogger());

            _productBl = productBl;

            /*_ecConfig = new ECConfig
            {
                ExceptionHandler = new ECExceptionHandler(new ECLogger()),
                Logger = new ECLogger()
            };*/
        }

        /// <summary>
        /// Authenticates the user and returns JWT.
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthenticateUser(string username, string password)
        {
            throw new NotImplementedException();
            /*try
            {
                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return _ecConfig.ExceptionHandler.ReturnHttpResponse(ex, Request);
            }*/
        }

        /// <summary>
        /// Returns available active products for the region.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult<List<DtoProduct>> GetAvailableProducts()
        {
            try
            {
                /*Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader);
                if (String.IsNullOrEmpty(authorizationHeader.ToString()))
                    throw new ECExceptionHttpResponse(new ObjectResult(null) { StatusCode = StatusCodes.Status401Unauthorized });*/
                _productBl.GetAvailableProducts();
                return null;
            }
            catch (Exception ex)
            {
                return _ecConfig.ExceptionHandler.ReturnHttpResponse(ex, Request);
            }
        }
    }
}