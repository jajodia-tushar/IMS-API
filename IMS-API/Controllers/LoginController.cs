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
        /// <remarks>
        /// Note that login contains username and password.
        /// 
        ///     POST 
        ///     {
        ///        "username":"string",
        ///        "password":"string"
        ///     }
        ///     
        /// </remarks>
        /// <returns>User Details and Access Token</returns>
        /// <response code="200">Returns valid user details and access token</response>
        /// <response code="401">If the username and password is invalid</response>

        // POST: api/Login
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(Response), 401)]
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
