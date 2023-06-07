#nullable disable
using Microsoft.EntityFrameworkCore;
using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL.Models;

namespace ECommerce.IdentityAPI.DAL
{
    public class UserDAL : IUserDAL
    {
        private const string _msgUserNotFound = "User not found.";

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
                    return EcCommon.Map<Models.User, DtoUser, List<Models.User>, List<DtoUser>>(dbc.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + EcCommon.getWholeException(ex));
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
                Models.User user = getUser(id);
                return EcCommon.Map<Models.User, DtoUser, Models.User, DtoUser>(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + id + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Adds a new User to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserDalDto object</returns>
        /// <exception cref="Exception"></exception>
        public DtoUser AddUser(DtoUser userDAL, List<int> lsIdRoles, int idUserAdmin)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    Models.User user = EcCommon.Map<DtoUser, Models.User, DtoUser, Models.User>(userDAL);
                    user.Uchanged = idUserAdmin;
                    user.Tchanged = DateTime.Now;
                    dbc.Users.Add(user);
                    dbc.SaveChanges();

                    if (lsIdRoles == null || lsIdRoles.Count == 0)
                    {
                        lsIdRoles = new List<int>();
                        lsIdRoles.Add(EcCommon.IdRole_Customer);
                    }
                    foreach (int idRole in lsIdRoles)
                    {
                        dbc.UserRoles.Add(new UserRole { IdRole = idRole, IdUser = user.IdUser, Ucreated = idUserAdmin, Tcreated = DateTime.Now });
                    }
                    dbc.SaveChanges();
                    return EcCommon.Map<Models.User, DtoUser, Models.User, DtoUser>(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + EcCommon.jsonSerializeIgnoreNulls(userDAL) + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Modifies the existing User. Throws Exception if not found.
        /// </summary>
        /// <param name="dtoUser"></param>
        /// <exception cref="Exception"></exception>
        public void ModifyUser(DtoUser dtoUser, int idUserAdmin)
        {
            try
            {
                if (dtoUser == null)
                    throw new Exception("Parameter cannot be null.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    if (getUser(dtoUser.IdUser) == null)
                        throw new Exception(_msgUserNotFound);
                    Models.User userModified = EcCommon.Map<DtoUser, Models.User, DtoUser, Models.User>(dtoUser);
                    var entry = dbc.Entry(userModified);
                    userModified.Uchanged = idUserAdmin;
                    userModified.Tchanged = DateTime.Now;
                    entry.Property(e => e.IdUser).IsModified = false;
                    entry.State = EntityState.Modified;
                    dbc.SaveChanges();
                }
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ModifyUser(" + EcCommon.jsonSerializeIgnoreNulls(dtoUser) + "," + idUserAdmin + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns roles for the specified user. If the user is not found an Exception is thrown.
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="ECException"></exception>
        /// <exception cref="Exception"></exception>
        public List<DtoRole> GetRolesForUser(int idUser)
        {
            try
            {
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    User user = getUser(idUser);
                    if (user == null)
                        throw new Exception(_msgUserNotFound);
                    List<Role> lsRole = dbc.UserRoles.Where(l => l.IdUser == idUser).Join(dbc.Roles, userRole => userRole.IdRole, role => role.IdRole,
                        (userRole, role) => new Role
                        {
                            IdRole = role.IdRole,
                            Name = role.Name
                        }).ToList();
                    return EcCommon.Map<Role, DtoRole, List<Role>, List<DtoRole>>(lsRole);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns a User from DB. If the object doesn't exist, it throws an Exception.
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Models.User getUser(int? idUser)
        {
            try
            {
                if (idUser == null)
                    throw new Exception("IdUser is null.");
                using (EcommerceContext dbc = new EcommerceContext())
                {
                    User user = dbc.Users.FirstOrDefault(l => l.IdUser == idUser);
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in findUser(" + idUser + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Finds a User object by username. If object not found, returns null.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DtoUser GetUserByUsername(string username)
        {
            try
            {
                if (String.IsNullOrEmpty(username))
                    throw new Exception("Username is empty.");

                using (EcommerceContext dbc = new EcommerceContext())
                {
                    User user = dbc.Users.FirstOrDefault(l => l.Username.ToLower() == username);
                    return EcCommon.Map<User, DtoUser>(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserByUsername('" + username + "'): " + EcCommon.getWholeException(ex));
            }

        }

        public DtoUser AddUser(DtoUser userDto, List<int> lsIdRoles)
        {
            throw new NotImplementedException();
        }
    }
}