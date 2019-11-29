

using IMS.DataLayer.Dal;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class LoginService : ILoginService
    {
        private IUserDbContext _userDbContext;
        private ITokenProvider _tokenProvider;
        
        public LoginService(IUserDbContext userDbContext,ITokenProvider tokenProvider)
        {
            _userDbContext = userDbContext;
            _tokenProvider = tokenProvider;
            
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            try { 
                    
                    if (loginRequest.Username == null || loginRequest.Password == null)
                    {
                        loginResponse.Status = Status.Failure;
                        loginResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.BadRequest,
                            ErrorMessage = Constants.ErrorMessages.MissingUsernameOrPassword
                        };
                        return loginResponse;

                    }
                    User user = _userDbContext.GetUserByCredintials(loginRequest.Username, loginRequest.Password);
                    if (user != null)
                    {
                        string token = _tokenProvider.GenerateToken(user);
                        loginResponse.Status = Status.Success;
                        loginResponse.AccessToken = token;
                        loginResponse.User = user;
                        
                    }
                    else
                    {
                        loginResponse.Status = Status.Failure;
                        loginResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.UnAuthorized,
                            ErrorMessage = Constants.ErrorMessages.InvalidUserNameOrPassword
                        };
                        
                    }
             }
            catch(Exception ex)
            {

                throw ex;

                
            }
           
            return loginResponse;

        }
    }
}
