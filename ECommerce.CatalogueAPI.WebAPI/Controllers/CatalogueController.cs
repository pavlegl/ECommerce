#nullable disable
using ECommerce.CatalogueAPI.BL;
using ECommerce.CatalogueAPI.Common;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ECommerce.CatalogueAPI.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        MainOptions _options;
        IECLogger _logger = null;
        IProductBL _productBl = null;
        BaseECExceptionHandler _excHandler = null;

        public CatalogueController(IOptionsSnapshot<MainOptions> options, IProductBL productBl, BaseECExceptionHandler excHandler, IECLogger logger)
        {
            _options = options.Get(MainOptions.OptionsName);
            _productBl = productBl;
            _logger = logger;
            _excHandler = excHandler;
        }

        /// <summary>
        /// Authenticates the user and returns JWT.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AuthenticateUser([FromBody] UserPass userPass)// [FromForm] string username, [FromForm] string password)
        {
            using (var client = new HttpClient())
            {
                //string sUrlIdentityApi = ECommerce.EcCommon.getConfigVar("UrlECommerceIdentityApi", _options, true);

                client.BaseAddress = new Uri(_options.UrlECommerceIdentityApi);// sUrlIdentityApi);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "CheckUserCredentialsCreateUserJwt");
                request.Content = new StringContent("{ \"UserName\":\"" + userPass.Username + "\", \"Password\":\"" + userPass.Password + "\"}",
                                                    Encoding.UTF8,
                                                    "application/json");

                var response = await client.SendAsync(request);
                string sResp = await response.Content.ReadAsStringAsync();
                return this.StatusCode((int)response.StatusCode, sResp);
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
                string sUrlIdentityApi = _options.UrlECommerceIdentityApi;
                SvcIdentityApi svc = new SvcIdentityApi(sUrlIdentityApi, httpClient);
                //......
                //......
                //......
            }
        }
    }
}