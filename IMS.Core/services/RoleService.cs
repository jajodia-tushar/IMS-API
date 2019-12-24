using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class RoleService : IRoleServcie
    {
        private IRoleDbContext _roleDbContext;
        private ITokenProvider _tokenProvider;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
     

        public RoleService(IRoleDbContext roleDbContext, ITokenProvider tokenProvider, ILogManager logger, IHttpContextAccessor httpContextAccessor)
        {
            _roleDbContext = roleDbContext;
            _tokenProvider = tokenProvider;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ListOfRolesResponse> GetAllRoles()
        {
            ListOfRolesResponse response = new ListOfRolesResponse
            {
                Status = Status.Failure
            };
            int requestedUserId = -1;
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
                requestedUserId = requestedUser.Id;
                response.Roles = await _roleDbContext.GetAllRoles();
                response.Status = Status.Success;
            }
            catch (CustomException e)
            {
                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "GetAllRoles", Severity.Medium, "Get Request", response); }).Start();
            }
            catch (Exception e)
            {

                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "GetAllRoles", IMS.Entities.Severity.Medium, "Roles", response); }).Start();
            }
            finally
            {
               
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Medium;

                new Task(() => { _logger.Log(null, response, "GetAllRoles", response.Status, severity, requestedUserId); }).Start();
            }
            return response;
        }
    }
}
