﻿

using IMS.DataLayer.Dal;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class LoginService : ILoginService
    {
        private IUserDbContext _userDbContext;
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogManager _logger;

        public LoginService(IUserDbContext userDbContext, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, ILogManager logger)
        {
            _userDbContext = userDbContext;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }

       
        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            User user = null;

            LoginResponse loginResponse = new LoginResponse();
            try
            {


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
                user = _userDbContext.GetUserByCredintials(loginRequest.Username, loginRequest.Password);
                if (user != null)
                {
                    string token =  _tokenProvider.GenerateToken(user);
                    DateTime  expirationTime = GetExpirationTime(user.Role.Name);
                  //  await _tokenProvider.StoreToken(token,expirationTime);
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
            catch(Exception e)
            {
                loginResponse.Status = Status.Failure;
                loginResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };

            }
            finally
            {
                Severity severity = Severity.No;
                if (loginResponse.Status == Status.Failure)
                    severity = Severity.Critical;

                new Task(() => { _logger.Log(loginRequest, loginResponse,"Login", loginResponse.Status, severity, -1); }).Start();
            }

            return loginResponse;

        }
        private DateTime GetExpirationTime(string role)
        {
            string rolename = role.ToLower();
            double minutes=Constants.Roles.ExpirationTimeInMinutes[rolename];
            DateTime expirationTime = DateTime.Now.AddMinutes(minutes);
            return expirationTime;
        }

    }
}
