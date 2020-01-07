using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationApi.Core;
using NotificationApi.Core.Models;

namespace Notification_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private IEmailService _emailService;
        public NotificationController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        // POST: api/Notification
        [HttpPost]
        public async Task<Response> SendNotification([FromBody] Email email)
        {
            Response response = null;
            try
            {
                 response = await _emailService.Send(email);
            }
            catch(Exception e)
            {
                response = new Response
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode=Constants.ErrorCodes.ServerError,
                        ErrorMessage=Constants.ErrorMessages.ServerError
                    }
                };
            }
            return response;
        }

    }
}
