using IMS.Contracts.Models;
using IMS.Contracts.Models.Responses;
using IMS.DataLayer.Dal;
using IMS.TokenManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class LoginService
    {
        private UserDal _userDal;
        private TokenService _tokenService;
        public LoginService(UserDal userDal,TokenService tokenService)
        {
            _userDal = userDal;
            _tokenService = tokenService;
        }

        public Response GetJwtToken(LoginRequest loginRequest)
        {           //validate
            if (loginRequest.Username == null || loginRequest.Password == null)
                return new FailureResponse("Missing Username/password ");

            User user = _userDal.GetUserByUsername(loginRequest.Username, loginRequest.Password);
            if (user != null)
            {
                string token=_tokenService.GenerateToken(user);
                return new SuccessResponse(token);//return token
            }
            else
            {
                return new FailureResponse("Invalid Credintials");

            }
        }
    }
}
