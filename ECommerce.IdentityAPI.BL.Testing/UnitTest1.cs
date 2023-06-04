using ECommerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ECommerce.IdentityAPI.BL.Testing
{
    public class Tests
    {
        DtoUserDal _userValid = null;

        [SetUp]
        public void Setup()
        {
            _userValid = new DtoUserDal
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
        }

        #region Test Username Format

        [Test]
        public void TestLongUsername()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.Username = "ml12345678901234567890123456789012345678901234567890";
                userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestShortUsername()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.Username = "ml";
                userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestUsernameWithNoDot()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.Username = "mladenkurpejovic";
                userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }

        [Test]
        public void TestUsernameWithNumberInWrongPlace()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.Username = "mladen3.kurpejovic";
                userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.UserNameNotProperlyFormated);
            }
        }
        #endregion

        [Test]
        public void TestPasswordFormat()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.Password = "123456";
                DtoUserDal userDAL = userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.PasswordNotProperlyFormated);
            }
        }

        [Test]
        public void TestEmailFormat()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                _userValid.EmailAddress = "noMonkey";
                DtoUserDal userDAL = userBl.AddUser(_userValid);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.EmailBadFormat);
            }
        }

        [Test]
        public void TestEmailAlreadyUsed()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            try
            {
                userBl.AddUser(_userValid);
                DtoUserDal userNew = Common.Map<DtoUserDal, DtoUserDal, DtoUserDal, DtoUserDal>(_userValid);
                userNew.EmailAddress = _userValid.EmailAddress;
                userNew.Username = "janko.serhatlic";
                userNew.FirstName = "Janko";
                userNew.LastName = "Serhatliæ";
                userBl.AddUser(userNew);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.EmailAlreadyUsed);
            }
        }

        [Test]
        public void TestValidCountry()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUserDal userMladen = new DtoUserDal
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
            try
            {
                DtoUserDal userDAL = userBl.AddUser(userMladen);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.CountryNotSupported);
            }
        }

        [Test]
        public void TestIsPostCodeProvidedWhenNeeded()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUserDal userMladen = new DtoUserDal
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
                DtoUserDal userDAL = userBl.AddUser(userMladen);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(ECException));
                Assert.AreEqual(((ECException)ex).ExceptionType, EnumExceptionType.UserMustProvidePostCode);
            }
        }

        [Test]
        public void TestDefaultRoleIsCustomer()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUserDal userNew = userBl.AddUser(_userValid);
            List<DtoRoleDal> lsRoles = userBl.GetRolesForUser(userNew.IdUser.Value);
            Assert.IsTrue(lsRoles.Any(l => l.IdRole == 3));
        }

        [Test]
        public void TestBanUser()
        {
            UserBL userBl = new UserBL(new FakeUserDal());
            DtoUserDal userMladen = new DtoUserDal
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
            DtoUserDal userDalDto = userBl.AddUser(userMladen);
            userBl.BanUser(userDalDto.IdUser.Value);
            userDalDto = userBl.GetUserById(userDalDto.IdUser.Value);
            Assert.IsTrue(userDalDto.IsOnHold);
        }

        [Test]
        public void ValidateJwtToken()
        {
            IECAuthContainerModel authContainerModel = new JwtContainerModel
            {
                Claims = new Claim[] {
                    new Claim(ClaimTypes.Name, "2"),
                    new Claim(ClaimTypes.Role, String.Concat(',',new[]{ 2, 3 }))
                },
                SecretKey = "VWJlckdpZ2FTZWtyZXRLbGp1YyM5MTg4MTMxMDI0MSE="
            };
            ECAuthService jwtService = new JwtService(authContainerModel);
            string token = jwtService.GenerateToken();

            if (jwtService.IsTokenValid(token))
            {
                List<Claim> lsClaims = jwtService.GetTokenClaims(token).ToList();
                Assert.AreEqual(lsClaims.FirstOrDefault(l => l.Type == ClaimTypes.Name).Value, "2");
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

    }
}