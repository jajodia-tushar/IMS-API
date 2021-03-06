

using IMS.DataLayer.Dal;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
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


                if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    loginResponse.Status = Status.Failure;
                    loginResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.MissingUsernameOrPassword
                    };
                    return loginResponse;

                }
                loginRequest.Password = Utility.Hash(loginRequest.Password);
                user =  await _userDbContext.GetUserByCredintials(loginRequest.Username, loginRequest.Password);
                if (user != null)
                {
                    DateTime expirationTime = GetExpirationTime(user.Role.Name);
                    string token =  await _tokenProvider.GenerateToken(user,expirationTime);
                    await _tokenProvider.StoreToken(token,expirationTime);
                    loginResponse.Status = Status.Success;
                    loginResponse.AccessToken = token;
                    loginResponse.User = user;

                }
                else
                {
                    loginResponse.Status = Status.Failure;
                    loginResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidUserNameOrPassword);                 

                }
            }
            catch(Exception exception)
            {
                loginResponse.Status = Status.Failure;
                loginResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Login", Severity.Critical, loginRequest, loginResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (loginResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                

                new Task(() => {
                                LoginResponse responseToLog = CloneOf(loginResponse);
                                responseToLog.AccessToken = null;
                                _logger.Log(loginRequest, responseToLog, "Login", loginResponse.Status, severity, -1);
                               }).Start();
            }

            return loginResponse;

        }

        private static LoginResponse CloneOf(LoginResponse loginResponse)
        {
            string response=(string)JsonConvert.SerializeObject(loginResponse).Clone();
            return JsonConvert.DeserializeObject<LoginResponse>(response);
        }

        public async Task<Response> Logout()
        {
            Response response = new Response
            {
                Status=Status.Failure
            };
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (isTokenPresentInHeader)
                {
                    string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    bool isTokenDeleted = await _tokenProvider.DeleteToken(token);
                    if (isTokenDeleted)
                        response.Status = Status.Success;
                }
                else
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);

               

            }
            catch (Exception exception)
            {
                response.Status = Status.Failure;
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                new Task(() => { _logger.LogException(exception, "Logout", Severity.Critical, null, response); }).Start();

            }
            return response;
        }

        private DateTime GetExpirationTime(string role)
        {
            string rolename = role.ToLower();
            double minutes=Constants.Roles.ExpirationTimeInMinutes[rolename];
            DateTime expirationTime = DateTime.Now.AddMinutes(minutes);
            return expirationTime;
        }

        public async Task<Response> UpdateUserPassword(int userId, ChangePasswordDetails changePasswordDetails)
        {
            Response response = new Response()
            {
                Status = Status.Failure
            };
            int requestedUserId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser = request.User;
                requestedUserId = requestedUser.Id;
                User user = await _userDbContext.GetUserById(userId);
                if(requestedUserId != userId || user == null)
                {
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized,Constants.ErrorMessages.UnAuthorized);
                    return response;
                }
                else
                {
                    if (user.IsDefaultPasswordChanged == true)
                    {
                        var hashOldPassword = Utility.Hash(changePasswordDetails.OldPassword);
                        User userByOldPassword = await _userDbContext.GetUserByCredintials(user.Username, hashOldPassword);
                        if (userByOldPassword == null || userByOldPassword.Password != user.Password)
                        {
                            response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.IncorrectOldPassword);
                            return response;
                        }
                    }
                    Validators.UserValidator.CheckPasswordFormat(changePasswordDetails.NewPassword);
                    var hashNewPassword = Utility.Hash(changePasswordDetails.NewPassword);
                    if (await _userDbContext.IsNewpasswordRepeated(requestedUserId, hashNewPassword))
                    {
                        response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.NewPasswordRepeated);
                        return response;
                    }
                    if (!await _userDbContext.UpdateUserPassword(requestedUserId, hashNewPassword))
                    {
                        response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.PasswordNotUpdated);
                        return response;
                    }
                    response.Status = Status.Success;
                }
            }
            catch (CustomException e)
            {
                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "UpdateUserPassword", Severity.Critical, requestedUserId, response); }).Start();

            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "UpdateUserPassword", Severity.Critical,requestedUserId, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(userId, response, "UpdateUserPassword", response.Status, severity, requestedUserId); }).Start();
            }
            return response;
        }
    }
}
