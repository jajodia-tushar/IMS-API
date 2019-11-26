using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
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
        

        // POST: api/Login
        [HttpPost]
        public Response Login([FromBody] LoginRequest login)
        {
            try
            {
                IMS.Entities.LoginRequest entityLoginRequest = Translator.ToEntitiesObject(login);
                IMS.Entities.LoginResponse entityLoginResponse = _loginService.Login(entityLoginRequest);
                IMS.Contracts.LoginResponse contractsLoginResponse = Translator.ToDataContractsObject(entityLoginResponse);
                return contractsLoginResponse;
            }
            catch
            {
                return new IMS.Contracts.LoginResponse()
                        {
                            Status = Status.Failure,
                            Error = new Error()
                            {
                                ErrorCode = 500,
                                ErrorMessage = "Internal server error"
                            }
                        };
            }
        }

       
    }
}
