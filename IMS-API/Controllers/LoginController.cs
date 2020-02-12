using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace IMS_API.Controllers
{
    
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService _loginService;
        private ILogManager _logger;
        public LoginController(ILoginService loginService, ILogManager logManager)
        {
            _loginService = loginService;
            this._logger = logManager;
        }

        /// <summary>
        /// Validates the user using the login credentials and returns user details and access token
        /// </summary>
        /// <param name="login">Here login contains two fields named username and password</param>
        /// <returns>User Details and Access Token</returns>
        /// <response code="200">Returns user details and access token if username and password is valid otherwise it returns null and status failure</response>

        // POST: api/Login
        [Route("api/[action]")]
        [HttpPost]
        public async Task<LoginResponse> Login([FromBody] LoginRequest login)
        {
            var a = this.HttpContext.Request.Headers["Authorization"];
            LoginResponse contractsLoginResponse = null;
            try
            {
                IMS.Entities.LoginRequest entityLoginRequest = LoginTranslator.ToEntitiesObject(login);
                IMS.Entities.LoginResponse entityLoginResponse = await _loginService.Login(entityLoginRequest);
                contractsLoginResponse = LoginTranslator.ToDataContractsObject(entityLoginResponse);
            }
            catch(Exception exception)
            {
                contractsLoginResponse = new IMS.Contracts.LoginResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Login", IMS.Entities.Severity.Critical, login, contractsLoginResponse); }).Start();
            }
            return contractsLoginResponse;

        }

        [Route("api/[action]")]
        [HttpDelete]
        public async Task<Response> Logout()
        {
            IMS.Contracts.Response contractsResponse = null;
            try
            {
                IMS.Entities.Response response = await _loginService.Logout();
                contractsResponse = Translator.ToDataContractsObject(response);
               
            }
            catch(Exception exception)
            {
               contractsResponse= new IMS.Contracts.LoginResponse()
               {
                   Status = Status.Failure,
                   Error = new Error()
                   {
                       ErrorCode = Constants.ErrorCodes.ServerError,
                       ErrorMessage = Constants.ErrorMessages.LogoutFailed
                   }
               };
                new Task(() => { _logger.LogException(exception, "Logout", IMS.Entities.Severity.Critical, null, contractsResponse); }).Start();
            }
            return contractsResponse;
        }


        [Route("api/Login/updateuserpassword/{userId:int}")]
        [Authorize(Roles = "Admin,SuperAdmin,Clerk,Shelf")]
        [HttpPatch]
        public async Task<Response> UpdateUserPassword(int userId, [FromBody] ChangePasswordDetails changePasswordDetails)
        {
            IMS.Contracts.Response contractsResponse = null;
            try
            {
                IMS.Entities.ChangePasswordDetails doChangePasswordDetails = LoginTranslator.ToEntitiesObject(changePasswordDetails);
                IMS.Entities.Response response = await _loginService.UpdateUserPassword(userId,doChangePasswordDetails);
                contractsResponse = Translator.ToDataContractsObject(response);

            }
            catch (Exception exception)
            {
                contractsResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "UpdateUserPassword", IMS.Entities.Severity.Critical,userId, contractsResponse); }).Start();
            }
            return contractsResponse;
        }

    }
}
