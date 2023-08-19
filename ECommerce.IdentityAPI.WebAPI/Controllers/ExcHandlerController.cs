#nullable disable
using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL.Models;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerce.IdentityAPI.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExcHandlerController : ControllerBase
    {
        IECLogger _logger = null;
        IUserBL _userBl = null;
        IECAuthService _authService = null;
        IHttpContextAccessor _httpContextAccessor = null;
        BaseECExceptionHandler _excHandler = null;

        public ExcHandlerController(IConfiguration config, IUserBL userBL, BaseECExceptionHandler excHandler, IECLogger logger, IECAuthService authService, IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _excHandler = excHandler;
            _logger = logger;
            _userBl = userBL;
            _httpContextAccessor = contextAccessor;
        }

        /*[Route("api/ExcHandler/HandleError")]*/
        [HttpGet]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var exc = context.Error;

            int statusCode = exc is ECException ? ((ECException)exc).StatusCode : StatusCodes.Status500InternalServerError;

            return StatusCode(statusCode, exc.Message);
        }
    }
}
