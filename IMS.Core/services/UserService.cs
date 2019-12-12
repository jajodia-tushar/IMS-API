using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task<UsersResponse> GetAllAdmins()
        {
            UsersResponse getAllAdminsResponse = new UsersResponse();
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
                        getAllAdminsResponse.Status = Status.Failure;
                        getAllAdminsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                        List<User> adminsList = await _userDbContext.GetAllAdmins();
                        if (adminsList != null)
                        {
                            getAllAdminsResponse.Status = Status.Success;
                            getAllAdminsResponse.Users = adminsList;
                        }
                        return getAllAdminsResponse;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    getAllAdminsResponse.Status = Status.Failure;
                    getAllAdminsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception ex)
            {
                getAllAdminsResponse.Status = Status.Failure;
                getAllAdminsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (getAllAdminsResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("Get All Admins", getAllAdminsResponse, "Get All Admins", getAllAdminsResponse.Status, severity, -1); }).Start();
            }
            return getAllAdminsResponse;
        }
    }
}

