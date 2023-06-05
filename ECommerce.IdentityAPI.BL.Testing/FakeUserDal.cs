using ECommerce.IdentityAPI.Common;
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
        private List<DtoUser> _reposUsers = new List<DtoUser>();
        private List<DtoUserRole> _reposUserRoles = new List<DtoUserRole>();
        private List<DtoRole> _reposRoles = new List<DtoRole>();

        public FakeUserDal()
        {
            _reposRoles.Add(new DtoRole { IdRole = 2, Name = "Administrator" });
            _reposRoles.Add(new DtoRole { IdRole = 3, Name = "Customer" });
        }

        public DtoUser AddUser(DtoUser userDto)
        {
            int maxId = _reposUsers.Max(l => l.IdUser) ?? 0;
            userDto.IdUser = maxId + 1;
            userDto.Uchanged = 1;
            userDto.Tchanged = DateTime.Now;
            _reposUsers.Add(userDto);
            _reposUserRoles.Add(new DtoUserRole { IdRole = 3, IdUser = userDto.IdUser.Value }); // <--- Adds a default role Customer.
            return userDto;
        }

        public bool CheckUserCredentialsGetIdUser(string userName, string passwordHash, ref int idUser)
        {
            DtoUser user = _reposUsers.FirstOrDefault(l => l.Username.ToLower() == userName && l.Password == passwordHash);
            if (user == null)
                return false;
            idUser = user.IdUser.Value;
            return true;
        }

        public List<DtoRole> GetRolesForUser(int idUser)
        {
            DtoUser userDto = GetUserById(idUser);
            return _reposUserRoles.Where(l => l.IdUser == idUser).Join(_reposRoles, userRole => userRole.IdRole, role => role.IdRole,
                (userRole, role) =>
                    new DtoRole
                    {
                        IdRole = role.IdRole,
                        Name = role.Name
                    }
                ).ToList();
        }

        public DtoUser GetUserById(int id)
        {
            return _reposUsers.Find(l => l.IdUser == id);
        }

        public List<DtoUser> GetUsers()
        {
            return _reposUsers.ToList();
        }

        public void ModifyUser(DtoUser userDto)
        {
            if (userDto == null)
                throw new Exception("userDto is null.");
            for (int i = 0; i < _reposUsers.Count; i++)
            {
                if (_reposUsers[i].IdUser == userDto.IdUser)
                    _reposUsers[i] = userDto;
            }
        }
    }
}
