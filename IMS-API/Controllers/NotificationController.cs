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
    public class NotificationController : ControllerBase
    {
        private IAdminNotificationService _adminNotificationService;
        public NotificationController(IAdminNotificationService adminNotificationService)
        {
            _adminNotificationService = adminNotificationService;
        }
        // GET: api/Notification
        [HttpGet]
        public async Task<NotificationResponse> Get()
        {
            var notificationResponse = new NotificationResponse();
            try
            {
                var doNotificationResponse = await _adminNotificationService.GetAdminNotifications();
                notificationResponse = NotificationTranslator.ToDataContractObject(doNotificationResponse);
            }
            catch(Exception exception)
            {
                notificationResponse.Status = Status.Failure;
                notificationResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
            }
            return notificationResponse;
        }

    }
}
