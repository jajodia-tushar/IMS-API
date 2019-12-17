using IMS.DataLayer.Interfaces;
using IMS.Entities;
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

        public UserService(IUserDbContext userDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._userDbContext = userDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
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
    }
}

