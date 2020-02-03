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
        private IUserService _userService;
        public UserServiceTests()
        {
            _userDbContext = new Mock<IUserDbContext>();
            _logger = new Mock<ILogManager>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _tokenProvider = new Mock<ITokenProvider>();
            _accessControlDbContext = new Mock<IAccessControlDbContext>();
            _userService= new UserService(_userDbContext.Object, _logger.Object, _tokenProvider.Object, _httpContextAccessor.Object, _accessControlDbContext.Object);
        }
         
        public void ValidateToken(bool isValidToken,string token)
        {
            _tokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(isValidToken));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + token;
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }
        //AddUser

        [Fact]
        public async void Add_NewUser_Returns_Frobiden_Error_And_Status_Failure_If_RequestedRole_Doest_Not_Access()
        {
            //Set Up
            User newUser = new User()
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
             _accessControlDbContext.Setup(a => a.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(false));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            UsersResponse response = await _userService.AddUser(newUser);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(403, response.Error.ErrorCode);
        }
        [Fact]
        public async void Add_NewUser_Returns_BadRequest_Error_And_Status_Failure_If_UserNameOrEmail_Repeated()
        {
            //Setup
            User newUser = new User()
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
             _accessControlDbContext.Setup(a => a.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(true));           
            _userDbContext.Setup(u => u.CheckEmailOrUserNameAvailability(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            UsersResponse response = await _userService.AddUser(newUser);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        
        [Fact]
        public async void Add_NewUser__Status_Success_If_Everything_Is_Correct()
        {
            //Setup
            User newUser = new User()
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
            _accessControlDbContext.Setup(a => a.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(true));
            _userDbContext.Setup(u => u.CheckEmailOrUserNameAvailability(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            _userDbContext.Setup(u => u.Save(It.IsAny<User>(),It.IsAny<int>(),It.IsAny<int>())).Returns(Task.FromResult(true));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            UsersResponse response = await _userService.AddUser(newUser);
            //Verify
            Assert.Equal(Status.Success, response.Status);
            Assert.True(newUser.Equals(response.Users[0]));
            
        }
        
        //CheckEmailAvilability

        [Fact]
        public async void Check_Email_Availability_Returns_Error_If_Email_Repeated()
        {
            //Setup
           _userDbContext.Setup(u => u.CheckEmailAvailability(It.IsAny<string>())).Returns(Task.FromResult(true));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.CheckEmailAvailability("vijay@tavisca.com");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest,response.Error.ErrorCode);
        }
        [Fact]
        public async void Check_Email_Availability_Returns_Success_If_Email_Not_Repeated()
        {
            //Setup
            _userDbContext.Setup(u => u.CheckEmailAvailability(It.IsAny<string>())).Returns(Task.FromResult(false));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.CheckEmailAvailability("vijay@tavisca.com");
            //Verify
            Assert.Equal(Status.Success, response.Status);
        }

        //CheckUsernameAvailability
        [Fact]
        public async void Check_Username_Availability_Returns_Error_If_Email_Repeated()
        {
            //Setup
            _userDbContext.Setup(u => u.CheckUserNameAvailability(It.IsAny<string>())).Returns(Task.FromResult(true));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.CheckUsernameAvailability("vijaymohan");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Check_Username_Availability_Returns_Success_If_Email_Not_Repeated()
        {
            //Setup
            _userDbContext.Setup(u => u.CheckUserNameAvailability(It.IsAny<string>())).Returns(Task.FromResult(false));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.CheckUsernameAvailability("vijaymohan");
            //Verify
            Assert.Equal(Status.Success, response.Status);
        }

        //GetUsersByRoleName
        [Fact]
        public async void Get_Users_By_RoleName_Gives_UsersNotFound_If_No_Users()
        {
            //Setup
            _userDbContext.Setup(u => u.GetUsersByRole(It.IsAny<string>())).Returns(Task.FromResult(new List<User>()));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.GetUsersByRole("Admin");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }
       
        [Fact]
        public async void Get_Users_By_RoleName_Gives_Users_If_Present()
        {
            //Setup
            _userDbContext.Setup(u => u.GetUsersByRole(It.IsAny<string>())).Returns(Task.FromResult(new List<User>() { new User()}));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.GetUsersByRole("Admin");
            //Verify
            Assert.Equal(Status.Success, response.Status);
            
        }
        //GetAllUsers
        [Fact]
        public async void Get_All_Users_By_RequestedRole_Gives_UsersNotFound_If_No_Users()
        {
            //Setup
            _userDbContext.Setup(u => u.GetAllUsers(It.IsAny<Role>())).Returns(Task.FromResult(new List<User>()));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.GetAllUsers();
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }
        [Fact]
        public async void Get_All_Users_By_RequestedRole_Gives_Users_If_Present()
        {
            //Setup
            _userDbContext.Setup(u => u.GetAllUsers(It.IsAny<Role>())).Returns(Task.FromResult(new List<User>() { new User() }));
            ValidateToken(true, Tokens.SuperAdmin);
            //Act
            Response response = await _userService.GetAllUsers();
            //Verify
            Assert.Equal(Status.Success, response.Status);
        }
        //update user
        [Fact]
        public async void Update_User_will_return_Not_Found_If_User_Not_Present()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            User updateUser = new User()
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
            _userDbContext.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(Task.FromResult((User)null));

            //Act
            UsersResponse response = await _userService.UpdateUser(updateUser,"name change");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }
        //update user
        [Fact]
        public async void Update_User_will_return_Unauthorized_If_RequestedUser_Does_Not_Access()
        {
            //Setup
            ValidateToken(true, Tokens.Clerk);
            User updateUser = new User()
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
            _userDbContext.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(Task.FromResult(new User() { Id = 1, Role = new Role { Id = 1, Name = "Admin" } }));
            _accessControlDbContext.Setup(u => u.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(false));
            //Act
            UsersResponse response = await _userService.UpdateUser(updateUser, "name change");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.UnAuthorized, response.Error.ErrorCode);
        }
        [Fact]
        public async void Update_User_will_return_Success_If_RequestedUser_has_Access()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            User updateUser = new User()
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
            _userDbContext.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(Task.FromResult(new User() { Id = 1, Role = new Role { Id = 1, Name = "Admin" } }));
            _accessControlDbContext.Setup(u => u.HasAccessControl(It.IsAny<Role>(), It.IsAny<Role>())).Returns(Task.FromResult(true));
            _userDbContext.Setup(u => u.UpdateUser(It.IsAny<User>())).Returns(Task.FromResult(updateUser));
            //Act
            UsersResponse response = await _userService.UpdateUser(updateUser, "name change");
            //Verify
            Assert.Equal(Status.Success, response.Status);
            Assert.True(updateUser.Equals(response.Users[0]));
        }
        [Fact]
        public async void Delete_User_will_return_Failure_If_User_Not_Found()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            User deleteUser = new User()
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
            _userDbContext.Setup(u => u.DeleteUser(It.IsAny<int>(),It.IsAny<bool>())).Returns(Task.FromResult(false));
            //Act
            Response response = await _userService.DeleteUser(deleteUser.Id,true, "User left out of company");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound,response.Error.ErrorCode);
        }
        [Fact]
        public async void Delete_User_will_return_SUccess_If_User_Is_Deleted()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            User deleteUser = new User()
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
            _userDbContext.Setup(u => u.DeleteUser(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            //Act
            Response response = await _userService.DeleteUser(deleteUser.Id, true, "User left out of company");
            //Verify
            Assert.Equal(Status.Success, response.Status);
            
        }

    }
    }
