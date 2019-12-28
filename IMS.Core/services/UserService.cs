using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class UserService : IUserService
    {
        private IUserDbContext _userDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        private IAccessControlDbContext _accessControlDbContext;

        public UserService(IUserDbContext userDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, IAccessControlDbContext accessControlDbContext)
        {
            this._userDbContext = userDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
            this._accessControlDbContext = accessControlDbContext;
        }
        public async Task<UsersResponse> GetUsersByRole(string roleName)
        { 
            UsersResponse usersResponse = new UsersResponse();
            if (String.IsNullOrEmpty(roleName))
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.MissingValues);
            }
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        usersResponse.Status = Status.Failure;
                        usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                        List<User> userList = await _userDbContext.GetUsersByRole(roleName);

                        if (userList.Count!=0)
                        {
                            usersResponse.Status = Status.Success;
                            usersResponse.Users = userList;
                        }
                        return usersResponse;
                    }
                    catch (Exception exception)
                    {
                        new Task(() => { _logger.LogException(exception, "GetUserRoleByName", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();
                    }
                }
                else
                {
                    usersResponse.Status = Status.Failure;
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetUserRoleByName", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(roleName, usersResponse, "Get Users By Role Name", usersResponse.Status, severity, -1); }).Start();
            }
            return usersResponse;
        }
        public async Task<UsersResponse> GetAllUsers()
        {
            UsersResponse usersResponse = new UsersResponse()
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser = Utility.GetUserFromToken(token);
                userId = requestedUser.Id;

                List<User> users = await _userDbContext.GetAllUsers(requestedUser.Role);
                if (users.Count == 0)
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                else
                {
                    usersResponse.Users = users;
                    usersResponse.Status = Status.Success;
                }



            }
            catch (CustomException e)
            {
                usersResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, " GetAllUsers", Severity.Medium, "Get Request", usersResponse); }).Start();
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetAllUsers", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("Get Request", usersResponse, "GetAllUsers", usersResponse.Status, severity, userId); }).Start();
            }

            return usersResponse;
        }
    }
}

