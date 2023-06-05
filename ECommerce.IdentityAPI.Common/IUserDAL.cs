namespace ECommerce.IdentityAPI.Common
{
    public interface IUserDAL
    {
        List<DtoUser> GetUsers();
        DtoUser GetUserById(int id);
        DtoUser AddUser(DtoUser userDto);
        List<DtoRole> GetRolesForUser(int idUser);
        void ModifyUser(DtoUser userDto);
        bool CheckUserCredentialsGetIdUser(string userName, string passwordHash, ref int idUser);
    }
}
