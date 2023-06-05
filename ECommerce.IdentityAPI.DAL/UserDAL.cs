using ECommerce.Models;
using ECommerce.IdentityAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.IdentityAPI.Common;

namespace ECommerce.IdentityAPI.DAL
{
    public class UserDAL : IUserDAL
    {
        //private EcommerceContext _dbc;

        public UserDAL()
        {
        }

        /// <summary>
        /// Retrieves all User entities.
        /// </summary>
        /// <returns>UserDalDto object.</returns>
        /// <exception cref="Exception"></exception>
        public List<DtoUser> GetUsers()
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    return ECommerce.Common.Map<Models.User, DtoUser, List<Models.User>, List<DtoUser>>(dbc.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns a User object by its id.
        /// </summary>
        /// <param name="id">Id of the User.</param>
        /// <returns>UserDalDto object.</returns>
        /// <exception cref="Exception"></exception>
        public DtoUser GetUserById(int id)
        {
            try
            {
                Models.User user = findUserThrowExcIfNotFound(id);
                return ECommerce.Common.Map<Models.User, DtoUser, Models.User, DtoUser>(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + id + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Adds a new User to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserDalDto object</returns>
        /// <exception cref="Exception"></exception>
        public DtoUser AddUser(DtoUser userDAL)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    Models.User user = ECommerce.Common.Map<DtoUser, Models.User, DtoUser, Models.User>(userDAL);
                    dbc.Users.Add(user);
                    dbc.SaveChanges();
                    return ECommerce.Common.Map<Models.User, DtoUser, Models.User, DtoUser>(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + ECommerce.Common.jsonSerializeIgnoreNulls(userDAL) + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Modifies the existing User.
        /// </summary>
        /// <param name="userDAL"></param>
        /// <exception cref="Exception"></exception>
        public void ModifyUser(DtoUser userDAL)
        {
            try
            {
                if (userDAL == null)
                    throw new Exception("Parameter cannot be null.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    Models.User userOrig = findUserThrowExcIfNotFound(userDAL.IdUser);
                    Models.User user = ECommerce.Common.Map<DtoUser, Models.User, DtoUser, Models.User>(userDAL);
                    //var entry = _dbc.Entry(userOrig);
                    dbc.Entry(userOrig).State = EntityState.Modified;
                    dbc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ModifyUser(" + ECommerce.Common.jsonSerializeIgnoreNulls(userDAL) + "): " + ECommerce.Common.getWholeException(ex));
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
                throw new Exception("Error in findUserReturn404(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }

            if (user == null)
                throw new Exception("User IdUser=" + idUser + " not found.");
            return user;
        }

        /// <summary>
        /// Checks the user credentials and returns true if they are correct.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passwordHash"></param>
        /// <param name="idUser">Value of idUser if the credentials are correct.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckUserCredentialsGetIdUser(string userName, string passwordHash, ref int idUser)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwordHash))
                    throw new Exception("Username or passwordHash is empty.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    User user = dbc.Users.FirstOrDefault(l => l.Username.ToLower() == userName.ToLower() && l.Password == passwordHash);
                    if (user == null)
                        return false;
                    idUser = user.IdUser;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckUserCredentialsGetIdUser('" + userName + "','" + passwordHash + "'): " + ECommerce.Common.getWholeException(ex));
            }
        }

        public List<DtoRole> GetRolesForUser(int idUser)
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
                    return ECommerce.Common.Map<Role, DtoRole, List<Role>, List<DtoRole>>(lsRole);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }
        }
    }
}