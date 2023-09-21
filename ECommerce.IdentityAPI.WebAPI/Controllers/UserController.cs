#nullable disable
using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL.Models;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerce.IdentityAPI.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserBL _userBl = null;
        IHttpContextAccessor _httpContextAccessor = null;
        BaseECExceptionHandler _excHandler = null;
        private IFixedGuidProvider _singleGuidProvider;

        public UserController(IConfiguration config, IUserBL userBL, BaseECExceptionHandler excHandler, IHttpContextAccessor contextAccessor, IFixedGuidProvider singleGuidProvider)
        {
            _excHandler = excHandler;
            _userBl = userBL;
            _httpContextAccessor = contextAccessor;
            _singleGuidProvider = singleGuidProvider;
        }

        /// <summary>
        /// Retrieves all User entities.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult<List<DtoUser>> GetUsers()
        {
            return Ok(_userBl.GetUsers());
        }

        /// <summary>
        /// Returns DtoUser object 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult<DtoUser> GetUserById(int id)
        {
            try
            {
                return Ok(_userBl.GetUserById(id));
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        /// <summary>
        /// Adds a new User to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult<DtoUser> Post([FromBody] DtoUser user)
        {
            try
            {
                int idAdmin = Int32.Parse(getClaimValue(ClaimTypes.NameIdentifier));
                return Ok(_userBl.AddUser(user, null, idAdmin));
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        /// <summary>
        /// Modifies the existing User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ECException"></exception>
        [HttpPut]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult Put([FromBody] DtoUser user)
        {
            try
            {
                int idAdmin = Int32.Parse(getClaimValue(ClaimTypes.NameIdentifier));
                _userBl.ModifyUser(user, idAdmin);
                return Ok();
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        [HttpPost]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult SetUserOnHold([FromForm] int idUser, [FromForm] bool isOnHold)
        {
            try
            {
                int idAdmin = Int32.Parse(getClaimValue(ClaimTypes.NameIdentifier));
                _userBl.PutUserOnHold(idUser, isOnHold, idAdmin);
                return Ok();
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        [HttpPost]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult<DtoUserRole> GetUserRoles([FromForm] int idUser)
        {
            try
            {
                return Ok(_userBl.GetRolesForUser(idUser));
            }
            catch (Exception ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        [HttpDelete]
        [Authorize(Policy = CustomAuthPolicies.IsAdmin)]
        public ActionResult Delete()
        {
            try
            {
                throw new ECException(StatusCodes.Status501NotImplemented, "To put a user on hold use PutUserOnHold(int idUser, true).");
            }
            catch (ECException ex)
            {
                return _excHandler.ReturnHttpResponse(ex, Request);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<string> CheckUserCredentialsCreateUserJwt([FromBody] UserPass userPass)
        {
            try
            {
                string jwt = _userBl.CheckUserCredentialsCreateUserJwt(userPass.Username, userPass.Password);
                return Ok(jwt);
            }
            catch (ECException ecex)
            {
                return _excHandler.ReturnHttpResponse(ecex, Request);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<IEnumerable<Claim>> CheckJwtReturnClaims([FromBody] string jwt)
        {
            try
            {
                IEnumerable<Claim> lsClaims = _userBl.CheckJwtReturnClaims(jwt);
                return Ok(lsClaims);
            }
            catch (ECException ecex)
            {
                return _excHandler.ReturnHttpResponse(ecex, Request);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public string TestSingleGuidProvider()
        {
            return _singleGuidProvider.GetGuid();
        }


        /// <summary>
        /// Returns custom Claim value from the authorized user.
        /// </summary>
        /// <param name="claimName">Name of the Claim.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string getClaimValue(string claimName)
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User.FindFirstValue(claimName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getClaimValue('" + claimName + "'): " + EcCommon.getWholeException(ex));
            }
        }

    }
}
