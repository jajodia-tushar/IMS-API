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
<<<<<<< HEAD
        /// returns all users based on the role name
        /// </summary>
        /// <param name="RoleName">Takes the name of the role corresponding to whihc we need users</param>
        /// <returns>all users of the role name along with status</returns>
        /// <response code="200">Returns users with status</response>
        [HttpGet("Role/{RoleName}",Name = "Get(string RoleName)")]
        public async Task<UsersResponse> GetUsersByRoleId(string RoleName)
        {
            UsersResponse contractUsers = null;
            try
            {
                IMS.Entities.UsersResponse entityUsers = await _userService.GetUsersByRole(RoleName);
                contractUsers = UserTranslator.ToDataContractsObject(entityUsers);
            }
            catch
            {
                contractUsers = new IMS.Contracts.UsersResponse()
=======
        /// returns all admins
        /// </summary>
        /// <returns>all admins along with status</returns>
        /// <response code="200">Returns all Admins</response>
        [HttpGet("GetAdmins",Name = "GetAdmins()")]
        public async Task<UsersResponse> GetAllAdmins()
        {
            UsersResponse contractsAdmins = null;
            try
            {
                IMS.Entities.UsersResponse entityAdmins = await _userService.GetAllAdmins();
                contractsAdmins = UserTranslator.ToDataContractsObject(entityAdmins);
            }
            catch
            {
                contractsAdmins = new IMS.Contracts.UsersResponse()
>>>>>>> Get All Admins Implemented.
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
<<<<<<< HEAD
            return contractUsers;
=======
            return contractsAdmins;
>>>>>>> Get All Admins Implemented.
        }
    }
}
