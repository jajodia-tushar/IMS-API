using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class UserServiceTests
    {
        private Mock<IUserDbContext> _userDbContext;
        private Mock<ILogManager> _logger;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ITokenProvider> _tokenProvider;
        private Mock<IAccessControlDbContext> _accessControlDbContext;
        public UserServiceTests()
        {
            _userDbContext = new Mock<IUserDbContext>();
            _logger = new Mock<ILogManager>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _tokenProvider = new Mock<ITokenProvider>();
            _accessControlDbContext = new Mock<IAccessControlDbContext>();
        }
        public static User newUser = new User
        {
            Id = 1,
            Firstname = "vijay",
            Username = "vijaymohan",
            Lastname = "mohan",
            Email = "preddy@tavisca.com",
            Password = "Admin@123",
            Role = new Role
            {
                Id = 1,
                Name = "Admin"
            }


        };
        [Fact]
        public async void Returns_Errors_And_Status_Failure_When_Id_Is_Null()
        {
            UserService userService = new UserService(_userDbContext.Object, _logger.Object, _tokenProvider.Object, _httpContextAccessor.Object, _accessControlDbContext.Object);
            _accessControlDbContext.Setup(a => a.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(false));
            _tokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));           
            var context = new DefaultHttpContext();            
            context.Request.Headers["Authorization"] = "bearer "+Tokens.SuperAdmin;
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            
            UsersResponse response = await userService.AddUser(newUser);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(403, response.Error.ErrorCode);
        }
    }
}
