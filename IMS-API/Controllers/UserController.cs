using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        [Authorize(Roles = "Admin,SuperAdmin")]
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

        // PUT: api/
        /// <summary>
        /// returns updated user
        /// </summary>
        /// <param name="user">Takes the user to be updated</param>
        /// <returns>Updated user</returns>
        /// <response code="200">Returns the updated user</response>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut(Name = "UpdateUser(User user)")]
        public async Task<UsersResponse> UpdateUser(User user)
        {
            UsersResponse contractUsersResponse = null;
            try
            {
                IMS.Entities.User userEntity = UserTranslator.ToEntitiesObject(user);
                IMS.Entities.UsersResponse entityUsersResponse = await _userService.UpdateUser(userEntity);
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
                new Task(() => { _logger.LogException(exception, "UpdateUser", IMS.Entities.Severity.Medium, "UpdateUser", contractUsersResponse); }).Start();
            }
            return contractUsersResponse;
        }
        // GET: api/
        /// <summary>
        /// returns all pending Approval users 
        /// </summary>
        /// <returns>all pending Approval users  </returns>
        /// <response code="200">Returns users with status</response>
        [HttpGet("ApproveAdmins")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<UsersResponse> GetAllPendingAdminApprovals()
        {
            UsersResponse contractUsersResponse = null;
            try
            {
                IMS.Entities.UsersResponse entityUsersResponse = await _userService.GetAllPendingAdminApprovals();
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
                new Task(() => { _logger.LogException(exception, "GetAllPendingApprovals", IMS.Entities.Severity.Medium, "user/pendingapprovals", contractUsersResponse); }).Start();
            }
            return contractUsersResponse;
        }
        // POST: api/user
        /// <summary>
        /// Creates New USer
        /// </summary>
        /// <param name="user">Here user contains all user details along with credintials</param>
        /// <returns>created user</returns>
        /// <response code="200">Returns created user with status</response>
        [HttpPost(Name = "AddNewUser")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<UsersResponse> AddUser([FromBody] User user)
        {
            IMS.Contracts.UsersResponse contractsUserResponse = null;
            try
            {
                IMS.Entities.User entitiesUser = UserTranslator.ToEntitiesObject(user);
                IMS.Entities.UsersResponse entitiesUserResponse = await _userService.AddUser(entitiesUser);
                contractsUserResponse = UserTranslator.ToDataContractsObject(entitiesUserResponse);
            }
            catch (Exception e)
            {
                contractsUserResponse = new UsersResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "Add USer", IMS.Entities.Severity.Medium, user, contractsUserResponse); }).Start();
            }
            return contractsUserResponse;

        }
        // PUT: api/
        /// <summary>
        /// returns approved admin
        /// </summary>
        /// <param name="userId">Takes the user to be approved</param>
        /// <returns>Updated user</returns>
        /// <response code="200">Returns the approved user</response>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("ApproveAdmin/{userId}",Name = "ApproveAdmin(int userId")]
        public async Task<UsersResponse> ApproveAdmin(int userId)
        {
            UsersResponse contractUsersResponse = null;
            try
            {
                IMS.Entities.UsersResponse entityUsersResponse = await _userService.ApproveAdmin(userId);
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
                new Task(() => { _logger.LogException(exception, "ApproveAdmin", IMS.Entities.Severity.Medium,userId, contractUsersResponse); }).Start();
            }
            return contractUsersResponse;
        }
        // DELETE: api/
        /// <summary>
        /// returns deletion status
        /// </summary>
        /// <param name="userId">Takes the id of the user to be deleted</param>
        /// <param name="isHardDelete">Values 1 or 0 corresponds to whether the deletion is a hard delete or a soft delete</param>
        /// <returns>deletion status</returns>
        /// <response code="200">deletion status</response>
        [Authorize(Roles="SuperAdmin")]
        [HttpDelete("{userId}",Name = "Delete(int UserId")]
        public async Task<Response> Delete(int userId,bool isHardDelete)
        {
            Response contractDeleteUsersResponse = null;
            try
            {
                IMS.Entities.Response deleteUserResponse = await _userService.DeleteUser(userId, isHardDelete);
                contractDeleteUsersResponse = Translator.ToDataContractsObject(deleteUserResponse);
            }
            catch (Exception exception)
            {
                contractDeleteUsersResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "DeleteUser", IMS.Entities.Severity.Medium,userId, contractDeleteUsersResponse); }).Start();
            }
            return contractDeleteUsersResponse;
        }
        // Get: api/user
        /// <summary>
        /// Validate username exist or not
        /// </summary>
        /// <param name="username">contains username added by user</param>
        /// <returns>response </returns>
        /// <response code="200">return response object</response>
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("UserName")]
        public async Task<Response> IsUsernameExists(string username)
        {
            Response dtoValidUsername = null;
            try
            {
                IMS.Entities.Response doValidUsernameResponse = await _userService.CheckUsernameAvailability(username);
                dtoValidUsername = Translator.ToDataContractsObject(doValidUsernameResponse);
            }
            catch (Exception exception)
            {
                dtoValidUsername = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetValidUsername", IMS.Entities.Severity.Critical, username, dtoValidUsername); }).Start();
            }
            return dtoValidUsername;
        }

        //[Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Email")]
        public async Task<Response> IsEmailExists(string email)
        {
            Response dtoValidEmail = null;
            try
            {
                IMS.Entities.Response doValidEmailResponse = await _userService.CheckEmailAvailability(email);
                dtoValidEmail = Translator.ToDataContractsObject(doValidEmailResponse);
            }
            catch (Exception exception)
            {
                dtoValidEmail = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "IsEmailExists", IMS.Entities.Severity.Critical, email, dtoValidEmail); }).Start();
            }
            return dtoValidEmail;
        }
    }
}