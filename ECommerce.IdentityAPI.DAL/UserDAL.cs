using ECommerce.Models;
using ECommerce.IdentityAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.IdentityAPI.DAL
{
    public class UserDAL : IUserDAL
    {
        //private EcommerceContext _dbc;
        public ECConfig _ecConfig;

        public UserDAL()
        {
            //_dbc = new EcommerceContext();
            _ecConfig = new ECConfig
            {
                ExceptionHandler = new ECExceptionHttpResponseHandler(new ECLogger()),
                Logger = new ECLogger()
            };
        }

        /// <summary>
        /// Retrieves all User entities.
        /// </summary>
        /// <returns>UserDalDto object.</returns>
        /// <exception cref="Exception"></exception>
        public List<DtoUserDal> GetUsers()
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    return Common.Map<Models.User, DtoUserDal, List<Models.User>, List<DtoUserDal>>(dbc.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns a User object by its id.
        /// </summary>
        /// <param name="id">Id of the User.</param>
        /// <returns>UserDalDto object.</returns>
        /// <exception cref="Exception"></exception>
        public DtoUserDal GetUserById(int id)
        {
            try
            {
                Models.User user = findUserThrowExcIfNotFound(id);
                return Common.Map<Models.User, DtoUserDal, Models.User, DtoUserDal>(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + id + "): " + Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Adds a new User to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserDalDto object</returns>
        /// <exception cref="Exception"></exception>
        public DtoUserDal AddUser(DtoUserDal userDAL)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    Models.User user = Common.Map<DtoUserDal, Models.User, DtoUserDal, Models.User>(userDAL);
                    dbc.Users.Add(user);
                    dbc.SaveChanges();
                    return Common.Map<Models.User, DtoUserDal, Models.User, DtoUserDal>(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + Common.jsonSerializeIgnoreNulls(userDAL) + "): " + Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Modifies the existing User.
        /// </summary>
        /// <param name="userDAL"></param>
        /// <exception cref="Exception"></exception>
        public void ModifyUser(DtoUserDal userDAL)
        {
            try
            {
                if (userDAL == null)
                    throw new Exception("Parameter cannot be null.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    Models.User userOrig = findUserThrowExcIfNotFound(userDAL.IdUser);
                    Models.User user = Common.Map<DtoUserDal, Models.User, DtoUserDal, Models.User>(userDAL);
                    //var entry = _dbc.Entry(userOrig);
                    dbc.Entry(userOrig).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ModifyUser(" + Common.jsonSerializeIgnoreNulls(userDAL) + "): " + Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns a User from DB. If the object doesn't exist, it throws an Exception.
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Models.User findUserThrowExcIfNotFound(int? idUser)
        {
            Models.User user = null;
            try
            {
                if (idUser == null)
                    throw new Exception("IdUser is null.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    user = dbc.Users.FirstOrDefault(l => l.IdUser == idUser);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in findUserReturn404(" + idUser + "): " + Common.getWholeException(ex));
            }

            if (user == null)
                throw new Exception("User IdUser=" + idUser + " not found.");
            return user;
        }

        public bool CheckUserCredentials(string userName, string passwordHash)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwordHash))
                    throw new Exception("Username or passwordHash is empty.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    return dbc.Users.Any(l => l.Username.ToLower() == userName.ToLower() && l.Password == passwordHash);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckUserCredentials('" + userName + "','" + passwordHash + "'): " + Common.getWholeException(ex));
            }
        }

        public List<DtoRoleDal> GetRolesForUser(int idUser)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    List<Role> lsRole = dbc.UserRoles.Where(l => l.IdUser == idUser).Join(dbc.Roles, userRole => userRole.IdRole, role => role.IdRole,
                        (userRole, role) => new Role
                        {
                            IdRole = role.IdRole,
                            Name = role.Name
                        }).ToList();
                    return Common.Map<Role, DtoRoleDal, List<Role>, List<DtoRoleDal>>(lsRole);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + Common.getWholeException(ex));
            }
        }
    }
}