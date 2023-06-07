namespace ECommerce.IdentityAPI.Common
{
    public interface IUserDAL
    {
        List<DtoUser> GetUsers();
        DtoUser GetUserById(int id);
        DtoUser GetUserByUsername(string userName);
        DtoUser AddUser(DtoUser userDto, List<int> lsIdRoles, int idUserAdmin);
        List<DtoRole> GetRolesForUser(int idUser);
        void ModifyUser(DtoUser userDto, int idUserAdmin);
    }
}
