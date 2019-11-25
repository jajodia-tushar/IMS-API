
using System;
using System.Collections.Generic;
using System.Text;




namespace IMS.Core.Translators
{
    public static class Translator
    {
        public static Contracts.Models.Response ToDataContractsObject(Entities.Response entityLoginResponse)
        {
            if(entityLoginResponse is Entities.SuccessResponse )
            {
                Entities.SuccessResponse entitySuccessResponse = (Entities.SuccessResponse)entityLoginResponse;
                return new Contracts.Models.SuccessResponse(entitySuccessResponse.Data);
            }
            else
            {
                Entities.FailureResponse entityFailureResponse = (Entities.FailureResponse)entityLoginResponse;
                return new Contracts.Models.FailureResponse(entityFailureResponse.ErrorMessage);

            }
        }

        /* public IMS.Entities.User ToEntityObject(User user)
{
}

public IMS.Entities.LoginRequest ToEntitiyObject(LoginRequest loginRequest)
{
    //TODO: Write translations here
}

public LoginResponse ToDataContractObject(IMS.Entities.LoginResponse loginResponse)
{
    //TODO: Write translations here

}

public User ToDataContractObject(IMS.Entities.User user)
{

}


public User TranslateToUser(UserDto userDto)
{
    return new User()
            {
                Id = userDto.UserId,
                Username = userDto.Username,
                Fullname = userDto.Firstname + userDto.Secondname,
                Role=userDto.Role
            };
}
*/
        public static Entities.LoginRequest ToEntitiesObject(Contracts.Models.LoginRequest contractsLoginRequest)
        {
            return new Entities.LoginRequest()
            {
                Username = contractsLoginRequest.Username,
                Password=contractsLoginRequest.Password

            };
        }
    }
}
