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
        public ExcHandlerController()
        {
        }

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
