using ECommerce.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
                throw new Exception("Error in GetUsers(): " + Common.getWholeException(ex));
            }
        }*/

        public List<DtoUserDal> GetUsers()
        {
            try
            {
                List<DtoUserDal> lsUser = _userDal.GetUsers();
                IECLogger logger = new ECLogger();
                foreach (var item in lsUser)
                {
                    logger.w2l(item.FirstName + " " + item.LastName, EnumTypeOfLog.Information, null, null);
                }
                return lsUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUsers(): " + Common.getWholeException(ex));
            }
        }

        public DtoUserDal GetUserById(int idUser)
        {
            try
            {
                DtoUserDal userDalDto = _userDal.GetUserById(idUser);
                return userDalDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetUserById(" + idUser + "): " + Common.getWholeException(ex));
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
                throw new Exception("Error in GetUserById(" + idUser + "): " + Common.getWholeException(ex));
            }
        }*/

        public DtoUserDal AddUser(DtoUserDal user)
        {
            try
            {
                Misc.validateUserForPost(user, _userDal);
                user.Password = Common.hashSha256(user.Password);
                DtoUserDal userNew = (DtoUserDal)_userDal.AddUser(user);
                return userNew;
            }
            catch (ECException ece)
            {
                throw ece;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in AddUser(" + Common.jsonSerializeIgnoreNulls(user) + "): " + Common.getWholeException(ex));
            }
        }

        public bool CheckUserCredentials(string userName, string password)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    throw new Exception("Username or password is empty.");
                return _userDal.CheckUserCredentials(userName, Common.hashSha256(password));
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CheckUserCredentials('" + userName + "','" + password + "'): " + Common.getWholeException(ex));
            }
        }

        public List<DtoRoleDal> GetRolesForUser(int idUser)
        {
            try
            {
                return _userDal.GetRolesForUser(idUser).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetRolesForUser(" + idUser + "): " + Common.getWholeException(ex));
            }
        }

        public void BanUser(int idUser)
        {
            try
            {
                DtoUserDal userDalDto = _userDal.GetUserById(idUser);
                userDalDto.IsOnHold = true;
                _userDal.ModifyUser(userDalDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BanUser(" + idUser + "): " + Common.getWholeException(ex));
            }
        }

        public string CreateToken(IECAuthContainerModel authContainerModel, ECAuthService authService)
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
                throw new Exception("Error in CreateToken(" + Common.jsonSerializeIgnoreNulls(authContainerModel) + "," + 
                    Common.jsonSerializeIgnoreNulls(authService) + "): " + Common.getWholeException(ex));
            }
        }



    }
}