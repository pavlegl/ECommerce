#nullable disable
using ECommerce.IdentityAPI.Common;
using ECommerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.BL.Testing
{
    public class Tests
    {
        DtoUser _userValidMne = null;
        DtoUser _userValidAus = null;

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

            _userValidAus = new DtoUser
            {
                Address = "Sydney, Blvd.",
                CountryAlpha3Code = "AUS",
                EmailAddress = "joe.perry@gmail.com",
                FirstName = "Joe",
                LastName = "Perry",
                IsOnHold = false,
                Password = "ValidPass!499",
                Postcode = "2000",
                Username = "joe.perry2"
            };

        }

        #region Test Username Format

        [Test]
        public void TestLongUsername()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.Username = "ml12345678901234567890123456789012345678901234567890";
                userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestShortUsername()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.Username = "ml";
                userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestUsernameWithNoDot()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.Username = "mladenkurpejovic";
                userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestUsernameWithNumberInWrongPlace()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.Username = "mladen3.kurpejovic";
                userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }
        #endregion

        [Test]
        public void TestPasswordFormat()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.Password = "123456";
                DtoUser userDAL = userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.PasswordNotProperlyFormated);
            }
        }

        [Test]
        public void TestEmailFormat()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValidMne.EmailAddress = "noMonkey";
                DtoUser userDAL = userBl.AddUser(_userValidMne);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.EmailBadFormat);
            }
        }

        [Test]
        public void TestEmailAlreadyUsed()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                userBl.AddUser(_userValidMne);
                DtoUser userNew = new DtoUser();
                userNew.EmailAddress = _userValidMne.EmailAddress;
                userNew.Username = "janko.serhatlic";
                userNew.FirstName = "Janko";
                userNew.LastName = "Serhatliæ";
                userBl.AddUser(userNew);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.EmailAlreadyUsed);
            }
        }

        [Test]
        public void TestValidCountry()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                DtoUser userDAL = userBl.AddUser(_userValidAus);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.CountryNotSupported);
            }
        }

        [Test]
        public void TestIsPostCodeProvidedWhenNeeded()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUser userJoe = new DtoUser
            {
                Address = "Liverpool, Blvd.",
                CountryAlpha3Code = "GBR",
                EmailAddress = "joe.perry@gmail.com",
                FirstName = "Joe",
                LastName = "Perry",
                IsOnHold = false,
                Password = "ValidPass!499",
                Postcode = "",
                Username = "joe.perry2"
            };
            try
            {
                DtoUser userDAL = userBl.AddUser(userJoe);
            }
            catch (ECException ex)
            {
                Assert.AreEqual(ex.ExceptionType, EnumExceptionType.UserMustProvidePostCode);
            }
        }

        [Test]
        public void TestDefaultRoleIsCustomer()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUser userNew = userBl.AddUser(_userValidMne);
            List<DtoRole> lsRoles = userBl.GetRolesForUser(userNew.IdUser.Value);
            Assert.IsTrue(lsRoles.Any(l => l.IdRole == 3));
        }

        [Test]
        public void TestBanUser()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUser userDalDto = userBl.AddUser(_userValidMne);
            userBl.BanUser(userDalDto.IdUser.Value);
            userDalDto = userBl.GetUserById(userDalDto.IdUser.Value);
            Assert.IsTrue(userDalDto.IsOnHold);
        }

        [Test]
        public void ValidateCredentials()
        {
            // ----- Arranging -----
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUser userDalDto = userBl.AddUser(_userValidMne);

            IECAuthContainerModel authContainerModel = new JwtContainerModel
            {
                SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature,
                SecretKey = "VWJlckdpZ2FTZWtyZXRLbGp1YyM5MTg4MTMxMDI0MSE="
            };

            // ----- Acting -----
            IECAuthService jwtService = new JwtService(authContainerModel);

            string jwt = null;
            bool isUserAuthorized = userBl.CheckUserCredentialsReturnJwt(_userValidMne.Username, _userValidMne.Password, jwtService, ref jwt);
            bool isValidToken = userBl.IsTokenValid(jwt, new JwtService(authContainerModel));

            // ----- Asserting -----
            Assert.IsTrue(isUserAuthorized);
            Assert.IsTrue(isValidToken);
        }

    }
}