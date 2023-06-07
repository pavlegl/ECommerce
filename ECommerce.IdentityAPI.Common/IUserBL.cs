using System.Security.Claims;

namespace ECommerce.IdentityAPI.Common
{
    public interface IUserBL
    {
        List<DtoUser> GetUsers();
        DtoUser GetUserById(int idUser);
        DtoUser AddUser(DtoUser user, List<int> lsIdRoles, int idUserAdmin);
        void ModifyUser(DtoUser user, int idUserAdmin);
        bool CheckUserCredentialsCreateUserJwt(string userName, string password, IECAuthService authService, ref string jwt);

        bool CheckJwtReturnClaims(string jwt, ECommerce.IECAuthService authService, ref IEnumerable<Claim> lsClaims);
        List<DtoRole> GetRolesForUser(int idUser);
        void PutUserOnHold(int idUser, bool isOnHold, int idAdminUser);
        string CreateToken(IECAuthContainerModel authContainerModel, IECAuthService authService);
    }
}
