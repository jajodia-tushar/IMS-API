
using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class Translator
    {
        public static Contracts.LoginResponse ToDataContractsObject(Entities.LoginResponse entityLoginResponse)
        {
            Contracts.LoginResponse loginResponse = new Contracts.LoginResponse();
            if (entityLoginResponse.Status == Entities.Status.Success)
            {
                loginResponse.Status = Contracts.Status.Success;
                loginResponse.AccessToken = entityLoginResponse.AccessToken;
                loginResponse.User = ToDataContractsObject(entityLoginResponse.User);
            }
            else
            {
                loginResponse.Status = Contracts.Status.Failure;
                loginResponse.Error = ToDataContractsObject(entityLoginResponse.Error); 
            }
            return loginResponse;

        }

       
        public static Contracts.Error ToDataContractsObject(Entities.Error error)
        {
            return new Contracts.Error()
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage
            };
        }

        public static Contracts.User ToDataContractsObject(Entities.User user)
        {
            return new Contracts.User()
            {
                Id = user.Id,
                Username = user.Username,
                Password = null,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = ToDataContractsObject(user.Role)

            };
        }

        public static Contracts.Role ToDataContractsObject(Entities.Role role)
        {
            return new Contracts.Role()
                       {
                           Id = role.Id,
                            Name = role.Name
                       };
        }
        public static Entities.LoginRequest ToEntitiesObject(Contracts.LoginRequest contractsLoginRequest)
        {
            return new Entities.LoginRequest()
            {
                Username = contractsLoginRequest.Username,
                Password = contractsLoginRequest.Password

            };
        }


     }
}
