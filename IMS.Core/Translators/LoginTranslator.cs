using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class LoginTranslator
    {
        public static Contracts.LoginResponse ToDataContractsObject(Entities.LoginResponse entityLoginResponse)
        {
            Contracts.LoginResponse loginResponse = new Contracts.LoginResponse();
            if (entityLoginResponse.Status == Entities.Status.Success)
            {
                loginResponse.Status = Contracts.Status.Success;
                loginResponse.AccessToken = entityLoginResponse.AccessToken;
                loginResponse.User = UserTranslator.ToDataContractsObject(entityLoginResponse.User);
            }
            else
            {
                loginResponse.Status = Contracts.Status.Failure;
                loginResponse.Error = Translator.ToDataContractsObject(entityLoginResponse.Error);
            }
            return loginResponse;

        }
       
        public static Entities.LoginRequest ToEntitiesObject(Contracts.LoginRequest contractsLoginRequest)
        {
            return new Entities.LoginRequest()
            {
                Username = contractsLoginRequest.Username,
                Password = contractsLoginRequest.Password

            };
        }

        public static Entities.ChangePasswordDetails ToEntitiesObject(Contracts.ChangePasswordDetails contractsChangePasswordDetails)
        {
            return new Entities.ChangePasswordDetails()
            {
                OldPassword = contractsChangePasswordDetails.OldPassword,
                NewPassword = contractsChangePasswordDetails.NewPassword
            };
        }
    }
}
