using ECommerce.Models;

namespace ECommerce
{
    public interface IUserDAL
    {
        List<DtoUserDal> GetUsers();
        DtoUserDal GetUserById(int id);
        DtoUserDal AddUser(DtoUserDal userDalDto);
        List<DtoRoleDal> GetRolesForUser(int idUser);
        void ModifyUser(DtoUserDal userDalDto);
        bool CheckUserCredentials(string userName, string passwordHash);
    }
}
