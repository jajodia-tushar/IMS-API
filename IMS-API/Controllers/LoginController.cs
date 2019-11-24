using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IMS.Contracts.Models;
using IMS.Core.services;
using IMS.Contracts.Models.Responses;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private LoginService _loginService;
        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }
        

        // POST: api/Login
        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest login)
        {
            Response response = _loginService.GetJwtToken(login);
            if (response.Status==Status.success)
                return Ok(response);
            else
                return BadRequest(response);

        }

       
    }
}
