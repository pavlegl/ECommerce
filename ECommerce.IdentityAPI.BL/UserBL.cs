using ECommerce.IdentityAPI.Common;
using ECommerce.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.BL
{
    public class UserBL
    {
        //"https://localhost:7002/swagger/v1/swagger.json"
        private IUserDAL _userDal;

        public UserBL(IUserDAL userDal)
        {
            _userDal = userDal;
        }

        /*public List<UserDAL> GetUsers()
        {
            try
            {
                using (ClientDal client = new ClientDal(_sUrlIdentityApiDal))
                {
                    List<UserDAL> lsUser = (List<UserDAL>)await client.Swc.GetUsersAsync();
                    IECLogger logger = new ECLogger();
                    foreach (var item in lsUser)
                    {
                        logger.w2l(item.FirstName + " " + item.LastName, EnumTypeOfLog.Information, null, null);
                    }
                    return lsUser;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + ECommerce.Common.getWholeException(ex));
            }
        }*/

        public List<DtoUser> GetUsers()
        {
            try
            {
                List<DtoUser> lsUser = _userDal.GetUsers();
                return lsUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + ECommerce.Common.getWholeException(ex));
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
                throw new Exception("Error in GetUserById(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /*
        public async Task<UserDAL> GetUserById(int idUser)
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

        public DtoUser AddUser(DtoUser user)
        {
            try
            {
                DtoUser userForDb = ECommerce.Common.Map<DtoUser, DtoUser, DtoUser, DtoUser>(user);
                Misc.validateNewUser(userForDb, _userDal);
                userForDb.Password = ECommerce.Common.hashSha256(userForDb.Password);
                DtoUser userNew = (DtoUser)_userDal.AddUser(userForDb);
                return userNew;
            }
            catch (ECException ece)
            {
                throw ece;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + ECommerce.Common.jsonSerializeIgnoreNulls(user) + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Checks credentials and if they are correct it returns the JWT created by the supplied IECAuthService.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authService">Instance of the IECAuthService prefilled with SecretKey and SecurityAlgorithm.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string CheckUserCredentialsReturnJwt(string userName, string password, IECAuthService authService)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    throw new Exception("Username or password is empty.");

                int idUser = 0;
                if (!_userDal.CheckUserCredentialsGetIdUser(userName, ECommerce.Common.hashSha256(password), ref idUser))
                    throw new UnauthorizedAccessException();
                DtoUser user = GetUserById(idUser);
                List<DtoRole> lsRoles = GetRolesForUser(idUser);

                IECAuthContainerModel model = authService.AuthContainerModel;
                if (model == null)
                    throw new Exception("AuthContainerModel property is null.");
                if (String.IsNullOrEmpty(model.SecretKey))
                    throw new Exception("AuthContainerModel.SecretKey must be set.");
                if (String.IsNullOrEmpty(model.SecurityAlgorithm))
                    throw new Exception("AuthContainerModel.SecurityAlgorithm must be set.");

                Claim[] arClaims = new Claim[] {
                        new Claim(ClaimTypes.Name, idUser.ToString()),
                        new Claim(ClaimTypes.Role, String.Concat(',', lsRoles.ToArray()))
                    };

                authService.AuthContainerModel.Claims = arClaims;
                return authService.GenerateToken();
            }
            catch (UnauthorizedAccessException uaexc)
            {
                throw uaexc;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckUserCredentialsReturnJwt('" + userName + "','" + password + "'): " + ECommerce.Common.getWholeException(ex));
            }
        }

        /// <summary>
        /// Validates a token parameter and returns true if the token is valid.
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool IsTokenValid(string jwt, IECAuthService authService)
        {
            if (string.IsNullOrEmpty(jwt))
                throw new Exception("Token is empty.");

            return authService.IsTokenValid(jwt);
        }

        public List<DtoRole> GetRolesForUser(int idUser)
        {
            try
            {
                return _userDal.GetRolesForUser(idUser).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        public void BanUser(int idUser)
        {
            try
            {
                DtoUser userDalDto = _userDal.GetUserById(idUser);
                userDalDto.IsOnHold = true;
                _userDal.ModifyUser(userDalDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BanUser(" + idUser + "): " + ECommerce.Common.getWholeException(ex));
            }
        }

        public string CreateToken(IECAuthContainerModel authContainerModel, IECAuthService authService)
        {
            try
            {
                if (authContainerModel == null || authService == null)
                    throw new Exception("Parameter authContainerModel or authService is null.");

                string jwtToken = authService.GenerateToken();
                if (!authService.IsTokenValid(jwtToken))
                    throw new UnauthorizedAccessException();
                return jwtToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateToken(" + ECommerce.Common.jsonSerializeIgnoreNulls(authContainerModel) + "," +
                    ECommerce.Common.jsonSerializeIgnoreNulls(authService) + "): " + ECommerce.Common.getWholeException(ex));
            }
        }



    }
}