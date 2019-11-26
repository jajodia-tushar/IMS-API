

using IMS.DataLayer.Dal;
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

        public LoginResponse Login(LoginRequest loginRequest)
        {
            try { 
                    LoginResponse loginResponse = new LoginResponse();
                    if (loginRequest.Username == null || loginRequest.Password == null)
                    {
                        loginResponse.Status = Status.Failure;
                        loginResponse.Error = new Error()
                        {
                            ErrorCode = 400,
                            ErrorMessage = "missing Username/Password"
                        };
                        return loginResponse;

                    }
                    User user = _userDal.GetUserByCredintials(loginRequest.Username, loginRequest.Password);
                    if (user != null)
                    {
                        string token = _tokenProvider.GenerateToken(user);
                        loginResponse.Status = Status.Success;
                        loginResponse.AccessToken = token;
                        loginResponse.User = user;
                        return loginResponse;
                    }
                    else
                    {
                        loginResponse.Status = Status.Failure;
                        loginResponse.Error = new Error()
                        {
                            ErrorCode = 401,
                            ErrorMessage = "Invalid Username/Password"
                        };
                        return loginResponse;
                    }
             }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
