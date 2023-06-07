#nullable disable
using ECommerce.IdentityAPI.BL;
using ECommerce.IdentityAPI.Common;
using ECommerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.Testing
{
    public class Tests
    {
        DtoUser _userValidMne = null;
        DtoUser _userValidGbr = null;
        int _idUserAdmin = 2;
        FakeUserDal _fakeUserDal = null;

        [SetUp]
        public void Setup()
        {
            _userValidMne = new DtoUser
            {
                Address = "Crnogorskih serdara, bb",
                CountryAlpha3Code = "MNE",
                EmailAddress = "mladen.kurpejovic@gmail.com",
                FirstName = "Mladen",
                LastName = "Kurpejoviæ",
                IsOnHold = false,
                Password = "ValidPass!19",
                Postcode = "81000",
                Username = "mladen.kurpejovic"
            };

            _userValidGbr = new DtoUser
            {
                Address = "Scarborough, Blvd.",
                CountryAlpha3Code = "GBR",
                EmailAddress = "joe.perry@gmail.com",
                FirstName = "Joe",
                LastName = "Perry",
                IsOnHold = false,
                Password = "ValidPass!499",
                Postcode = "2000",
                Username = "joe.perry2"
            };

            _fakeUserDal = new FakeUserDal();
            _fakeUserDal.AddUser(new DtoUser
            {
                IdUser = 2,
                FirstName = "Administrator",
                LastName = "User",
                Username = "admin.user"
            }, new List<int> { EcCommon.IdRole_Admin }, 2);

        }

        [Test]
        public void TestGetUsers()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            // ----- Act -----
            userBl.AddUser(_userValidMne, null, _idUserAdmin);
            userBl.AddUser(_userValidGbr, null, _idUserAdmin);
            // ----- Assert -----
            List<DtoUser> lsUsers = userBl.GetUsers();
            Assert.That(lsUsers.Count(), Is.EqualTo(3));  // <--- 2 + Administrator.
        }

        [Test]
        public void TestAddUserSuccess()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            // ----- Act -----
            userBl.AddUser(_userValidMne, null, _idUserAdmin);
            // ----- Assert -----
            List<DtoUser> lsUsers = userBl.GetUsers();
            Assert.That(lsUsers.Count(), Is.EqualTo(2));  // <--- 2 + Administrator.
        }

        #region Test Username Format

        [Test]
        public void TestLongUsername()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.Username = "ml12345678901234567890123456789012345678901234567890";
                // ----- Act -----
                userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestShortUsername()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.Username = "ml";
                // ----- Act -----
                userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestUsernameWithNoDot()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.Username = "mladenkurpejovic";
                // ----- Act -----
                userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestUsernameWithNumberInWrongPlace()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.Username = "mladen3.kurpejovic";
                // ----- Act -----
                userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }
        #endregion

        [Test]
        public void TestPasswordFormat()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.Password = "123456";
                // ----- Act -----
                DtoUser userDAL = userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestEmailFormat()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidMne.EmailAddress = "noMonkey";
                // ----- Act -----
                DtoUser userDAL = userBl.AddUser(_userValidMne, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestEmailAlreadyUsed()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            DtoUser userWithEmailConflicted = EcCommon.Map<DtoUser, DtoUser>(_userValidMne);
            userWithEmailConflicted.Username = "janko.serhatlic";
            userWithEmailConflicted.FirstName = "Janko";
            userWithEmailConflicted.LastName = "Serhatliæ";
            try
            {
                // ----- Act -----
                userBl.AddUser(_userValidMne, null, _idUserAdmin);
                userBl.AddUser(userWithEmailConflicted, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
            }
        }

        [Test]
        public void TestValidCountry()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            try
            {
                _userValidGbr.CountryAlpha3Code = "AUS";
                // ----- Act -----
                DtoUser userDAL = userBl.AddUser(_userValidGbr, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestIsPostCodeProvidedWhenNeeded()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            _userValidGbr.Postcode = String.Empty;
            try
            {
                // ----- Act -----
                DtoUser userDAL = userBl.AddUser(_userValidGbr, null, _idUserAdmin);
                Assert.Fail();
            }
            catch (ECException ex)
            {
                // ----- Assert -----
                Assert.That(ex.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            }
        }

        [Test]
        public void TestDefaultRoleIsCustomer()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);

            // ----- Act -----
            DtoUser userNew = userBl.AddUser(_userValidMne, null, _idUserAdmin);
            List<DtoRole> lsRoles = userBl.GetRolesForUser(userNew.IdUser);

            // ----- Assert -----
            Assert.IsTrue(lsRoles.Any(l => l.IdRole == 3));
        }

        [Test]
        public void TestBanUser()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);

            // ----- Act -----
            DtoUser userDalDto = userBl.AddUser(_userValidMne, null, _idUserAdmin);
            userBl.PutUserOnHold(userDalDto.IdUser, true, 2);
            userDalDto = userBl.GetUserById(userDalDto.IdUser);

            // ----- Assert -----
            Assert.IsTrue(userDalDto.IsOnHold);
        }

        [Test]
        public void TestModifyUser()
        {
            // ----- Arrange -----
            string sNewAddress = "New Test address.";
            UserBL userBl = new UserBL(_fakeUserDal);

            // ----- Act -----
            DtoUser dtoUser = userBl.AddUser(_userValidMne, null, _idUserAdmin);
            dtoUser.Address = sNewAddress;
            userBl.ModifyUser(dtoUser, _idUserAdmin);
            DtoUser dtoUserLastVer = userBl.GetUserById(dtoUser.IdUser);

            // ----- Assert -----
            Assert.That(sNewAddress, Is.EqualTo(dtoUserLastVer.Address));
        }

        [Test]
        public void ValidateCredentials()
        {
            // ----- Arrange -----
            UserBL userBl = new UserBL(_fakeUserDal);
            DtoUser userDalDto = userBl.AddUser(_userValidMne, null, _idUserAdmin);

            IECAuthContainerModel authContainerModel = new JwtContainerModel
            {
                SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature,
                SecretKeyBase64 = "VWJlckdpZ2FTZWtyZXRLbGp1YyM5MTg4MTMxMDI0MSE="
            };

            // ----- Act -----
            IECAuthService jwtService = new JwtService(authContainerModel);

            string jwt = null;
            bool isUserAuthorized = userBl.CheckUserCredentialsCreateUserJwt(_userValidMne.Username, _userValidMne.Password, jwtService, ref jwt);
            IEnumerable<Claim> lsClaims = null;
            bool isValidToken = userBl.CheckJwtReturnClaims(jwt, new JwtService(authContainerModel), ref lsClaims);

            // ----- Assert -----
            Assert.IsTrue(isUserAuthorized);
            Assert.IsTrue(isValidToken);
        }

    }
}