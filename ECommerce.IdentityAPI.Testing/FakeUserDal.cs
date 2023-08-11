#nullable disable
using ECommerce.IdentityAPI.Common;
using ECommerce.Models;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.IdentityAPI.Testing
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

        public DtoUser AddUser(DtoUser userDto, List<int> lsIdRoles, int idUserAdmin)
        {
            if (userDto.IdUser == 0)
            {
                int maxId = _reposUsers.Count > 0 ? _reposUsers.Max(l => l.IdUser) : 0;
                userDto.IdUser = maxId + 1;
            }
            if (lsIdRoles == null || lsIdRoles.Count == 0)
            {
                lsIdRoles = new List<int>();
                lsIdRoles.Add(EcCommon.IdRole_Customer);
            }
            userDto.Uchanged = 2;
            userDto.Tchanged = DateTime.Now;
            _reposUsers.Add(userDto);
            foreach (int idRole in lsIdRoles)
                _reposUserRoles.Add(new DtoUserRole { IdRole = idRole, IdUser = userDto.IdUser }); // <--- Adds a default role Customer.
            return ECommerce.EcCommon.Map<DtoUser, DtoUser>(userDto);
        }

        public bool CheckUserCredentialsGetIdUser(string userName, string passwordHash, ref int idUser)
        {
            DtoUser user = _reposUsers.FirstOrDefault(l => l.Username.ToLower() == userName && l.Password == passwordHash);
            if (user == null)
                return false;
            idUser = user.IdUser;
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

        public DtoUser GetUserByUsername(string userName)
        {
            return _reposUsers.Find(l => l.Username == userName);
        }

        public List<DtoUser> GetUsers()
        {
            return _reposUsers.ToList();
        }

        public void ModifyUser(DtoUser userDto, int idUserAdmin)
        {
            if (userDto == null)
                throw new Exception("userDto is null.");

            for (int i = 0; i < _reposUsers.Count; i++)
            {
                if (_reposUsers[i].IdUser == userDto.IdUser)
                {
                    userDto.Uchanged = idUserAdmin;
                    userDto.Tchanged = DateTime.Now;
                    _reposUsers[i] = userDto;
                }
            }
        }
    }
}
