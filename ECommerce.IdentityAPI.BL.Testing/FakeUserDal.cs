using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.IdentityAPI.BL.Testing
{
    internal class FakeUserDal : IUserDAL
    {
        private List<DtoUserDal> _reposUsers = new List<DtoUserDal>();
        private List<DtoUserRoleDal> _reposUserRoles = new List<DtoUserRoleDal>();
        private List<DtoRoleDal> _reposRoles = new List<DtoRoleDal>();

        public FakeUserDal()
        {
            _reposRoles.Add(new DtoRoleDal { IdRole = 2, Name = "Administrator" });
            _reposRoles.Add(new DtoRoleDal { IdRole = 3, Name = "Customer" });
        }

        public DtoUserDal AddUser(DtoUserDal userDalDto)
        {
            int maxId = _reposUsers.Max(l => l.IdUser) ?? 0;
            userDalDto.IdUser = maxId + 1;
            userDalDto.Uchanged = 1;
            userDalDto.Tchanged = DateTime.Now;
            _reposUsers.Add(userDalDto);
            _reposUserRoles.Add(new DtoUserRoleDal { IdRole = 3, IdUser = userDalDto.IdUser.Value }); // <--- Adds a default role Customer.
            return userDalDto;
        }

        public bool CheckUserCredentials(string userName, string passwordHash)
        {
            return true;
        }

        public List<DtoRoleDal> GetRolesForUser(int idUser)
        {
            DtoUserDal userDalDto = GetUserById(idUser);
            return _reposUserRoles.Where(l => l.IdUser == idUser).Join(_reposRoles, userRole => userRole.IdRole, role => role.IdRole,
                (userRole, role) =>
                    new DtoRoleDal
                    {
                        IdRole = role.IdRole,
                        Name = role.Name
                    }
                ).ToList();
        }

        public DtoUserDal GetUserById(int id)
        {
            return _reposUsers.Find(l => l.IdUser == id);
        }

        public List<DtoUserDal> GetUsers()
        {
            return _reposUsers.ToList();
        }

        public void ModifyUser(DtoUserDal userDalDto)
        {
            if (userDalDto == null)
                throw new Exception("UserDalDto is null.");
            for (int i = 0; i < _reposUsers.Count; i++)
            {
                if (_reposUsers[i].IdUser == userDalDto.IdUser)
                    _reposUsers[i] = userDalDto;
            }
        }
    }
}
