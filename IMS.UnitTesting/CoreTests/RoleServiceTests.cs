using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class RoleServiceTests
    {
        public Mock<IRoleDbContext> _moqRoleDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;

        public RoleServiceTests()
        {
            _moqRoleDbContext = new Mock<IRoleDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public async void GetAllRoles_Should_Return_Valid_Response_And_Return_All_Roles()
        {
            var _moqUtility = new Mock<Utility>();
            _moqRoleDbContext.Setup(m =>m.GetAccessibleRoles(new Role() { Id = 1, Name = "Admin" })).Returns(GetAllRolesForRoleId1());
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            _moqUtility.Setup(m => m.GetUserFromToken(It.IsAny<string>())).Returns(new User());
            var roleServiceObject = new RoleService(_moqRoleDbContext.Object, _moqTokenProvider.Object,_moqLogManager.Object, _moqHttpContextAccessor.Object);
            var resultant = await roleServiceObject.GetAllRoles();
            Assert.Equal(Status.Success, resultant.Status);
        }

        [Fact]
        public async void GetAllRoles_Should_Return_Error_When_Token_Is_Invalid()
        {
            var _moqUtility = new Mock<Utility>();
            _moqRoleDbContext.Setup(m => m.GetAccessibleRoles(new Role() { Id = 1, Name = "Admin" })).Returns(GetAllRolesForRoleId1());
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _moqUtility.Setup(m => m.GetUserFromToken(It.IsAny<string>())).Returns(new User());
            var roleServiceObject = new RoleService(_moqRoleDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object);
            var resultant = await roleServiceObject.GetAllRoles();
            Assert.Equal(Status.Failure, resultant.Status);
        }
        private async Task<List<Role>> GetAllRolesForRoleId1()
        {
            return new List<Role>()
            {
                new Role()
                {
                    Id=1,
                    Name="Admin"
                },
                new Role()
                {
                    Id=2,
                    Name="Clerk"
                },
                new Role()
                {
                    Id=3,
                    Name="Shelf"
                }
            };
        }
    }
}