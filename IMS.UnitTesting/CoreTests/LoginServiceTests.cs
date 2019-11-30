using System;
using System.Collections.Generic;
using System.Text;
using IMS.Core;
using IMS.Entities.Interfaces;
using Moq;
using Xunit;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;

namespace IMS.UnitTest.CoreTests
{
    public class LoginServiceTests
    {
        public Mock<IUserDbContext> _moqUserDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public LoginServiceTests()
        {
            _moqUserDbContext = new Mock<IUserDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
        }

        [Fact]
        public void Returns_Errors_And_Status_Failure_When_Username_And_Password_Is_Null()
        {
            var loginRequest = GetLoginRequestWithNullValues();
            var loginServiceObject = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object);
            var response = loginServiceObject.Login(loginRequest);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.MissingUsernameOrPassword, response.Error.ErrorMessage);
        }

        [Fact]
        public void Returns_Errors_And_Status_Failure_When_Username_And_Password_Is_Wrong_And_User_Data_Is_Null()
        {
            var loginRequest = GetValidLoginRequest();
            User user = null;
            _moqUserDbContext.Setup(m => m.GetUserByCredintials(It.Is<String>(s => s.Equals(loginRequest.Username)), It.Is<String>(s => s.Equals(loginRequest.Password)))).Returns(user);
            var loginService = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object);
            var response = loginService.Login(loginRequest);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.UnAuthorized, response.Error.ErrorCode);
        }

        [Fact]
        public void Returns_Token_And_Status_Success_Login_Service_When_All_User_Data_Is_Valid()
        {
            var loginRequest = GetValidLoginRequest();
            User user = GetUserDetails();
            string token = "abcdefghijklmnopqrstuvwxyz";
            _moqUserDbContext.Setup(m => m.GetUserByCredintials(It.Is<String>(s => s.Equals(loginRequest.Username)), It.Is<String>(s => s.Equals(loginRequest.Password)))).Returns(user);
            _moqTokenProvider.Setup(m => m.GenerateToken(user)).Returns(token);
            var loginService = new LoginService(_moqUserDbContext.Object, _moqTokenProvider.Object);
            var response = loginService.Login(loginRequest);
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