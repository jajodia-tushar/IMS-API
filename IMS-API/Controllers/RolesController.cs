using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private IRoleService _roleService;
        private IMS.Logging.ILogManager _logger;
        public RolesController(IRoleService userService, IMS.Logging.ILogManager logManager)
        {
            _roleService = userService;
            _logger = logManager;
        }

        // GET: api/
        /// <summary>
        /// returns all roles
        /// </summary>     
        /// <returns>allRolesalong with status</returns>
        /// <response code="200">Returns Roles with status</response>
        [HttpGet]
        [Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<RolesResponse> GetAllRoles()
        {
            RolesResponse contractsRolesResponse = null;
            try
            {
                IMS.Entities.RolesResponse entityRolesResponse = await _roleService.GetAllRoles();
                contractsRolesResponse = RoleTranslator.ToDataContractsObject(entityRolesResponse);
            }
            catch (Exception exception)
            {
                contractsRolesResponse = new IMS.Contracts.RolesResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetAllRoles", IMS.Entities.Severity.Medium, "Roles", contractsRolesResponse); }).Start();
            }
            return contractsRolesResponse;
        }

    }
}