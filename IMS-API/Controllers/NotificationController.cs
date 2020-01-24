using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class NotificationController : ControllerBase
    {
        private IAdminNotificationService _adminNotificationService;
        public NotificationController(IAdminNotificationService adminNotificationService)
        {
            _adminNotificationService = adminNotificationService;
        }
        // GET: api/Notification
        [HttpGet]
        public async Task<NotificationResponse> Get(int? pageNumber, int? pageSize)
        {
            var notificationResponse = new NotificationResponse();
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;
                if (currentPageNumber <= 0 || currentPageSize <= 0)
                {
                    return new NotificationResponse()
                    {
                        Status = Status.Failure,
                        Error = new Error
                        {
                            ErrorCode = Constants.ErrorCodes.BadRequest,
                            ErrorMessage = Constants.ErrorMessages.InvalidPagingDetails
                        }
                    };
                }
                var doNotificationResponse = await _adminNotificationService.GetAdminNotificationsAsync(currentPageNumber, currentPageSize);
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
