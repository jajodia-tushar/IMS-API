using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }
        // GET: api/
        /// <summary>
        /// returns all admins
        /// </summary>
        /// <returns>all admins along with status</returns>
        /// <response code="200">Returns all Admins</response>
        [HttpGet("Role/{RoleId}",Name = "Get(int RoleId)")]
        public async Task<UsersResponse> GetUsersByRoleId(int RoleId)
        {
            UsersResponse contractUsers = null;
            try
            {
                IMS.Entities.UsersResponse entityUsers = await _userService.GetUsersByRole(RoleId);
                contractUsers = UserTranslator.ToDataContractsObject(entityUsers);
            }
            catch
            {
                contractUsers = new IMS.Contracts.UsersResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return contractUsers;
        }
    }
}
