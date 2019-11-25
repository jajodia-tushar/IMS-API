using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts.Models;
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
        public IActionResult Post([FromBody] LoginRequest login)
        {
            IMS.Entities.LoginRequest entityLoginRequest= Translator.ToEntitiesObject(login);
            IMS.Entities.Response entityLoginResponse = _loginService.Login(entityLoginRequest);
            IMS.Contracts.Models.Response contractsLoginResponse = Translator.ToDataContractsObject(entityLoginResponse);
            if (contractsLoginResponse.Status == IMS.Contracts.Models.Status.Success)
                return Ok(contractsLoginResponse);
            else
                return Unauthorized(contractsLoginResponse);
        }

       
    }
}
