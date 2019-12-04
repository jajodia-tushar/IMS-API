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
    public class LoginController : ControllerBase
    {
        private ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
        /// Validates the user using the login credentials and returns user details and access token
        /// </summary>
        /// <param name="login">Here login contains two fields named username and password</param>
        /// <returns>User Details and Access Token</returns>
        /// <response code="200">Returns user details and access token if username and password is valid otherwise it returns null and status failure</response>

        // POST: api/Login
        [HttpPost]
        public async Task<LoginResponse> Login([FromBody] LoginRequest login)
        {
            var a = this.HttpContext.Request.Headers["Authorization"];
            LoginResponse contractsLoginResponse = null;
            try
            {
                IMS.Entities.LoginRequest entityLoginRequest = Translator.ToEntitiesObject(login);
                IMS.Entities.LoginResponse entityLoginResponse = await _loginService.Login(entityLoginRequest);
                contractsLoginResponse = Translator.ToDataContractsObject(entityLoginResponse);
            }
            catch
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
            }
            return contractsLoginResponse;

        }
        
       
    }
}
