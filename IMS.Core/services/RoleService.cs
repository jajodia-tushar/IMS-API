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
            int userId =-1;
            try
            {
                response.Roles = await _roleDbContext.GetAllRoles();
                response.Status = Status.Success;
            }
            catch(Exception e)
            {

                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "GetAllRoles", IMS.Entities.Severity.Medium, "Get Request", response); }).Start();
            }
            finally
            {
               
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Medium;

                new Task(() => { _logger.Log(null, response, "Delete", response.Status, severity, userId); }).Start();
            }
            return response;
        }
    }
}
