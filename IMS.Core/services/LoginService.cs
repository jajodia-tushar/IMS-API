

using IMS.DataLayer.Dal;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.TokenManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class LoginService : ILoginService
    {
        private IUserDal _userDal;
        private ITokenProvider _tokenProvider;
        public LoginService(IUserDal userDal,ITokenProvider tokenProvider)
        {
            _userDal = userDal;
            _tokenProvider = tokenProvider;
        }

        public Response Login(LoginRequest loginRequest)
        {
            if (loginRequest.Username == null || loginRequest.Password == null)
                return new FailureResponse("missing Username/Password");
            User user = _userDal.GetUserByCredintials(loginRequest.Username, loginRequest.Password);
            if(user!=null)
            {
                string token = _tokenProvider.GenerateToken(user);
                return new SuccessResponse(token);
            }
            else
            {
                return new FailureResponse("Invalid Username/Password");
            }
        }
    }
}
