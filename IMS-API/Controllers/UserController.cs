using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private ILogManager _logger;
        public UserController(IUserService userService, ILogManager logManager)
        {
            this._userService = userService;
            this._logger = logManager;
        }
        // GET: api/
        /// <summary>
        /// returns all users based on the role name
        /// </summary>
        /// <param name="roleName">Takes the name of the role corresponding to which we need users</param>
        /// <returns>all users of the role name along with status</returns>
        /// <response code="200">Returns users with status</response>
        [HttpGet("Role/{roleName}", Name = "Get(string roleName)")]
        public async Task<UsersResponse> GetUsersByRoleName(string roleName)
        {
            UsersResponse contractUsersResponse = null;
            try
            {
                IMS.Entities.UsersResponse entityUsersResponse = await _userService.GetUsersByRole(roleName);
                contractUsersResponse = UserTranslator.ToDataContractsObject(entityUsersResponse);
            }
            catch (Exception exception)
            {
                contractUsersResponse = new IMS.Contracts.UsersResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetUsersRoleByName", IMS.Entities.Severity.Medium, "Get Request", contractUsersResponse); }).Start();
            }
            return contractUsersResponse;
        }
        // GET: api/
        /// <summary>
        /// returns all users 
        /// </summary>
        /// <returns>all users </returns>
        /// <response code="200">Returns users with status</response>
        [HttpGet]
        public async Task<UsersResponse> GetAllUsers()
        {
            UsersResponse contractUsersResponse = null;
            try
            {
                IMS.Entities.UsersResponse entityUsersResponse = await _userService.GetAllUsers();
                contractUsersResponse = UserTranslator.ToDataContractsObject(entityUsersResponse);
            }
            catch (Exception exception)
            {
                contractUsersResponse = new IMS.Contracts.UsersResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetAllUsers", IMS.Entities.Severity.Medium, "Get Request", contractUsersResponse); }).Start();
            }
            return contractUsersResponse;
        }
    }
}
