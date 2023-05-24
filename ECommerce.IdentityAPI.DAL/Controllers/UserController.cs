using ECommerce.IdentityAPI.DAL.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerce.IdentityAPI.DAL.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private EcommerceContext _dbc;

        public UserController(EcommerceContext dbc)
        {
            _dbc = dbc;
        }

        /// <summary>
        /// Retrieves all User entities.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return Ok(_dbc.Users.ToList());
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            try
            {
                User user = _dbc.Users.FirstOrDefault(l => l.IdUser == id);
                if (user == null)
                    throw new ECException(NotFound(id));
                return user;
            }
            catch (Exception ex)
            {
                return Common.HandleException(ex);
            }
        }

        /// <summary>
        /// Adds a new User to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post(User user)
        {
            try
            {
                /*User userAdminDefault = _dbc.Users.FirstOrDefault()
                user.Uchanged=*/
                _dbc.Users.Add(user);
                _dbc.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return Common.HandleException(ex);
            }
        }

        /// <summary>
        /// Modifies the existing User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ECException"></exception>
        [HttpPut]
        public ActionResult Put(User user)
        {
            try
            {
                if (user == null)
                    throw new Exception("Parameter cannot be null.");
                User userOrig = _dbc.Users.FirstOrDefault(l => l.IdUser == user.IdUser);
                if (userOrig == null)
                    throw new ECException(NotFound(user.IdUser));
                _dbc.Entry(userOrig).State = EntityState.Modified;
                _dbc.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return Common.HandleException(ex);
            }
        }

        [HttpPost]
        public ActionResult SwtUserOnHold(int idUser, bool isUserOnHold)
        {
            try
            {
                User user = _dbc.Users.FirstOrDefault(l => l.IdUser == idUser);
                if (user == null)
                    throw new ECException(NotFound(idUser));
                user.IsOnHold = isUserOnHold;
                _dbc.Entry(user).State = EntityState.Modified;
                _dbc.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return Common.HandleException(ex);
            }
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            try
            {
                throw new ECException("Not supported. To put a user on hold use SetUserOnHold(int idUser, true).");
            }
            catch (Exception ex)
            {
                return Common.HandleException(ex);
            }
        }
    }
}
