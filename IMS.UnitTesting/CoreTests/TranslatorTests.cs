using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using IMS.Core.Translators;

namespace IMS.UnitTest.CoreTests
{
    public class TranslatorTests
    {
        [Fact]
        public void Converting_Contract_Type_Object_To_Entitiy_Type_Object_Successfully()
        {
            Entities.LoginRequest expected = new Entities.LoginRequest() { Username = "admin", Password = "admin123" };
            Entities.LoginRequest resultant = Translator.ToEntitiesObject(new Contracts.LoginRequest() { Username = "admin", Password = "admin123" });
            Assert.Equal(expected.Username, resultant.Username);
            Assert.Equal(expected.Password, resultant.Password);
        }
        [Fact]
        public void Converting_Entity_Type_Role_To_Contract_Type_Role_Successfully()
        {
            Contracts.Role expected = new Contracts.Role() { Id = 1, Name = "admin" };
            Contracts.Role resultant = Translator.ToDataContractsObject(new Entities.Role() { Id = 1, Name = "admin" });
            Assert.Equal(expected.Id, resultant.Id);
            Assert.Equal(expected.Name, resultant.Name);
        }
        [Fact]
        public void Converting_Entity_Type_User_To_Contract_Type_User_Successfully()
        {
            Contracts.User expected = GetUserDetailsFromContractType();
            Contracts.User resultant = Translator.ToDataContractsObject(GetUserDetailsFromEntityType());
            Assert.Equal(expected.Id, resultant.Id);
            Assert.Equal(expected.Username, resultant.Username);
            Assert.Equal(expected.Password, resultant.Password);
        }
        [Fact]
        public void Converting_Entity_Type_Error_To_Contract_Type_Error_Successfully()
        {
            Contracts.Error expected = new Contracts.Error() { ErrorCode = 404, ErrorMessage = "Invalid Employee Id" };
            Contracts.Error resultant = Translator.ToDataContractsObject(new Entities.Error() { ErrorCode = 404, ErrorMessage = "Invalid Employee Id" });
            Assert.Equal(expected.ErrorCode, resultant.ErrorCode);
            Assert.Equal(expected.ErrorMessage, resultant.ErrorMessage);
        }
        [Fact]
        public void Converting_Entity_Type_Response_To_Contract_Type_Response_Successfully()
        {
            Contracts.LoginResponse expected = new Contracts.LoginResponse()
            {
                Status = Contracts.Status.Success,
                AccessToken = "abcdefghijklmnopqrstuvwxyz",
                User = GetUserDetailsFromContractType()
            };
            Contracts.LoginResponse resultant = Translator.ToDataContractsObject(new Entities.LoginResponse()
            {
                Status = Entities.Status.Success,
                AccessToken = "abcdefghijklmnopqrstuvwxyz",
                User = GetUserDetailsFromEntityType()
            });
            Assert.Equal(expected.AccessToken, resultant.AccessToken);
            Assert.Equal(expected.User.Id, resultant.User.Id);
        }

        public Entities.User GetUserDetailsFromEntityType()
        {
            return new Entities.User()
            {
                Id = 1,
                Username = "admin",
                Password = "admin123",
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
        public Contracts.User GetUserDetailsFromContractType()
        {
            return new Contracts.User()
            {
                Id = 1,
                Username = "admin",
                Password = null,
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                Email = "rochitaggarwal54@gmail.com",
                Role = new Contracts.Role()
                {
                    Id = 1,
                    Name = "admin"
                }
            };
        }
    }
}