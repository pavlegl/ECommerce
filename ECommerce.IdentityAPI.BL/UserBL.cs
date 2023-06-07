using ECommerce.IdentityAPI.Common;
using ECommerce.IdentityAPI.DAL;
using ECommerce.IdentityAPI.DAL.Models;
using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.BL
{
    public class UserBL : IUserBL
    {
        private IUserDAL _userDal;
        private const string _msgUserNotFound = "The user was not found.";

        public UserBL(IUserDAL userDal)
        {
            _userDal = userDal;
        }

        public List<DtoUser> GetUsers()
        {
            try
            {
                List<DtoUser> lsUser = _userDal.GetUsers();
                return lsUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + EcCommon.getWholeException(ex));
            }
        }

        public DtoUser GetUserById(int idUser)
        {
            try
            {
                DtoUser userDalDto = _userDal.GetUserById(idUser);
                return userDalDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + idUser + "): " + EcCommon.getWholeException(ex));
            }
        }

        public DtoUser AddUser(DtoUser user, List<int> lsIdRoles, int idUserAdmin)
        {
            try
            {
                DtoUser dtoUserForDb = EcCommon.Map<DtoUser, DtoUser, DtoUser, DtoUser>(user);
                Misc.validateNewUser(dtoUserForDb, _userDal);
                dtoUserForDb.Password = EcCommon.hashSha256(dtoUserForDb.Password);
                DtoUser userNew = (DtoUser)_userDal.AddUser(dtoUserForDb, lsIdRoles, idUserAdmin);
                userNew.Password = "---";
                return userNew;
            }
            catch (ECException ece)
            {
                throw ece;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + EcCommon.jsonSerializeIgnoreNulls(user) + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Checks credentials and if they are correct it returns the JWT created by the supplied IECAuthService.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="authService">IECAuthService for creating a JWT token.</param>
        /// <param name="jwt">JWT token</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckUserCredentialsCreateUserJwt(string username, string password, IECAuthService authService, ref string jwt)
        {
            try
            {
                if (String.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new ECException(StatusCodes.Status401Unauthorized, "Username or password is empty.");

                DtoUser dtoUser = _userDal.GetUserByUsername(username);
                if (dtoUser.IsOnHold || (dtoUser.Password != EcCommon.hashSha256(password)))
                    return false;

                List<DtoRole> lsRoles = GetRolesForUser(dtoUser.IdUser);

                IECAuthContainerModel model = authService.AuthContainerModel;
                if (model == null)
                    throw new Exception("AuthContainerModel property is null.");
                if (String.IsNullOrEmpty(model.SecretKeyBase64))
                    throw new Exception("AuthContainerModel.SecretKey must be set.");
                if (String.IsNullOrEmpty(model.SecurityAlgorithm))
                    throw new Exception("AuthContainerModel.SecurityAlgorithm must be set.");

                Claim[] arClaims = new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, dtoUser.IdUser.ToString()),
                        new Claim(CustomClaims.Roles, String.Join(',', lsRoles.Select(l=>l.IdRole))),
                        new Claim(CustomClaims.IsAdmin, lsRoles.Any(l => l.IdRole == EcCommon.IdRole_Admin).ToString().ToLower()),
                        new Claim(CustomClaims.IsCustomer, lsRoles.Any(l => l.IdRole == EcCommon.IdRole_Customer).ToString().ToLower())
                    };

                authService.AuthContainerModel.Claims = arClaims;
                jwt = authService.GenerateToken();
                return true;
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckUserCredentialsReturnJwt('" + username + "','" + password + "'): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Validates a token parameter and returns list of Claim if the token is valid.
        /// </summary>
        /// <param name="jwt">Jwt token for validation.</param>
        /// <returns>Indicator of successful validation.</returns>
        /// <exception cref="Exception"></exception>
        public bool CheckJwtReturnClaims(string jwt, IECAuthService authService, ref IEnumerable<Claim> lsClaims)
        {
            try
            {
                if (string.IsNullOrEmpty(jwt))
                    throw new Exception("Jwt token is empty.");
                if (!authService.IsTokenValid(jwt))
                    return false;
                lsClaims = authService.GetTokenClaims(jwt);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckJwtReturnClaims('" + jwt + "'," + EcCommon.jsonSerializeIgnoreNulls(authService) + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Returns all roles for the given active user.
        /// </summary>
        /// <param name="idUser">Id of the User.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DtoRole> GetRolesForUser(int idUser)
        {
            try
            {
                DtoUser user = _userDal.GetUserById(idUser);
                if (user == null)
                    throw new ECException(StatusCodes.Status404NotFound, _msgUserNotFound);
                return _userDal.GetRolesForUser(idUser).ToList();
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Bans the user.
        /// </summary>
        /// <param name="idUser">Id of user who is banned.</param>
        /// <param name="idUserAdmin">Id of user who does the action.</param>
        /// <exception cref="ECException"></exception>
        /// <exception cref="Exception"></exception>
        public void PutUserOnHold(int idUser, bool isOnHold, int idUserAdmin)
        {
            try
            {
                if (!isUserAdmin(idUserAdmin))
                    throw new ECException(StatusCodes.Status401Unauthorized, "The user performing the action doesn't have an Admin role.");
                DtoUser dtoUser = _userDal.GetUserById(idUser);
                if (dtoUser == null)
                    throw new ECException(StatusCodes.Status404NotFound, _msgUserNotFound);

                dtoUser.IsOnHold = isOnHold;
                _userDal.ModifyUser(dtoUser, idUserAdmin);
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BanUser(" + idUser + "," + idUserAdmin + "): " + EcCommon.getWholeException(ex));
            }
        }

        public string CreateToken(IECAuthContainerModel authContainerModel, IECAuthService authService)
        {
            try
            {
                if (authContainerModel == null || authService == null)
                    throw new Exception("Parameter authContainerModel or authService is null.");

                return authService.GenerateToken();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateToken(" + EcCommon.jsonSerializeIgnoreNulls(authContainerModel) + "," +
                    EcCommon.jsonSerializeIgnoreNulls(authService) + "): " + EcCommon.getWholeException(ex));
            }
        }

        /// <summary>
        /// Checks if user has Admin role and is active.
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="ECException"></exception>
        /// <exception cref="Exception"></exception>
        private bool isUserAdmin(int idUser)
        {
            try
            {
                DtoUser user = _userDal.GetUserById(idUser);
                if (user == null || user.IsOnHold)
                    throw new ECException(StatusCodes.Status404NotFound, "Admin user doesn't exist or is banned.");
                List<DtoRole> lsRoles = _userDal.GetRolesForUser(idUser);
                if (lsRoles.Select(l => l.IdRole).Contains(EcCommon.IdRole_Admin))
                    return true;
                return false;
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in isUserAdmin(" + idUser + "): " + EcCommon.getWholeException(ex));
            }
        }

        public void ModifyUser(DtoUser dtoUser, int idUserAdmin)
        {
            try
            {
                if (dtoUser == null)
                    throw new ECException(StatusCodes.Status404NotFound, "User parameter is null.");
                if (!isUserAdmin(idUserAdmin))
                    throw new ECException(StatusCodes.Status401Unauthorized, "The user performing the action doesn't have an Admin role.");
                DtoUser dtoUserDb = _userDal.GetUserById(dtoUser.IdUser);
                if (dtoUserDb == null)
                    throw new ECException(StatusCodes.Status404NotFound, _msgUserNotFound);

                _userDal.ModifyUser(dtoUser, idUserAdmin);
            }
            catch (ECException ecex)
            {
                throw ecex;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ModifyUser(" + EcCommon.jsonSerializeIgnoreNulls(dtoUser)
                    + "," + idUserAdmin + "): " + EcCommon.getWholeException(ex));
            }
        }

        /*public async Task<UserDAL> GetUserById(int idUser)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var client = new swaggerClient(_sUrlIdentityApiDal, httpClient);
                    UserDAL user = (UserDAL)await client.GetUserByIdAsync(idUser);
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }
        }*/

    }
}