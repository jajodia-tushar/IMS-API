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

        public async Task<UsersResponse> GetUsersByRole(string RoleName)
        {
            UsersResponse getUsersResponse = new UsersResponse();
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
                        getUsersResponse.Status = Status.Failure;
                        getUsersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                        List<User> adminsList = await _userDbContext.GetUsersByRole(RoleName);
                        if (adminsList.Count!=0)
                        {
                            getUsersResponse.Status = Status.Success;
                            getUsersResponse.Users = adminsList;
                        }
                        return getUsersResponse;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    getUsersResponse.Status = Status.Failure;
                    getUsersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception ex)
            {
                getUsersResponse.Status = Status.Failure;
                getUsersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (getUsersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(RoleName, getUsersResponse, "Get Users By Id", getUsersResponse.Status, severity, -1); }).Start();
            }
            return getUsersResponse;
        }
    }
}

