using System;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using IMS.DataLayer.Interfaces;
using IMS.Core.services;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Core;
using IMS.Logging;
using System.Threading.Tasks;

namespace IMS.UnitTest.CoreTests
{
    public class LoginServiceTests
    {
        public Mock<IUserDbContext> _moqUserDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;

        public LoginServiceTests()
        {
            _moqUserDbContext = new Mock<IUserDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();

            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();

        }

        [Fact]
        public async void Returns_Errors_And_Status_Failure_When_Username_And_Password_Is_Null()
        {
            var loginRequest = GetLoginRequestWithNullValues();

            var loginServiceObject = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object,_moqHttpContextAccessor.Object, _moqLogManager.Object);
            var response =  await loginServiceObject.Login(loginRequest);

            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.MissingUsernameOrPassword, response.Error.ErrorMessage);
        }

        [Fact]
        public async void Returns_Errors_And_Status_Failure_When_Username_And_Password_Is_Wrong_And_User_Data_Is_Null()
        {
            var loginRequest = GetValidLoginRequest();
            User user = null;
          //  _moqUserDbContext.Setup(m => m.GetUserByCredintials(It.Is<String>(s => s.Equals(loginRequest.Username)), It.Is<String>(s => s.Equals(loginRequest.Password)))).Returns(user);
           var loginService = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object,_moqHttpContextAccessor.Object, _moqLogManager.Object);
            var response = await loginService.Login(loginRequest);

            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.UnAuthorized, response.Error.ErrorCode);
        }

        [Fact]
        public async void Returns_Token_And_Status_Success_Login_Service_When_All_User_Data_Is_Valid()
        {
            var loginRequest = GetValidLoginRequest();
            User user = GetUserDetails();
            string token = "abcdefghijklmnopqrstuvwxyz";
            _moqUserDbContext.Setup(m => m.GetUserByCredintials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(user));
            _moqTokenProvider.Setup(m => m.GenerateToken(user, It.IsAny<DateTime>())).Returns(Task.FromResult(token));
            var loginService = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var response = await loginService.Login(loginRequest);

            Assert.Equal(Status.Success, response.Status);
            Assert.Equal(token, response.AccessToken);
        }

        private LoginRequest GetValidLoginRequest()
        {
            return new LoginRequest()
            {
                Username = "admin",
                Password = "admin123"
            };
        }
        private LoginRequest GetLoginRequestWithNullValues()
        {
            return new LoginRequest()
            {
                Username = null,
                Password = null
            };
        }
        private User GetUserDetails()
        {
            return new User()
            {
                Id = 1,
                Username = "admin",
                Password = null,
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                Email = "rochitaggarwal54@gmail.com",
                Role = new Entities.Role()
                {
                    Id = 1,
                    Name = "admin"
                }
            };
        }
    }
}