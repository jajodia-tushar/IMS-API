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
    public class RoleService : IRoleService
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

        public async Task<RolesResponse> GetAllRoles()
        {
            RolesResponse response = new RolesResponse
            {
                Status = Status.Failure
            };
            int requestedUserId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser =request.User;
                requestedUserId = requestedUser.Id;
                response.Roles = await _roleDbContext.GetAccessibleRoles(requestedUser.Role);
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
