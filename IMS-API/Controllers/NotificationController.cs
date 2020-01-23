using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        // GET: api/Notification
        [HttpGet]
        public async Task<NotificationResponse> Get()
        {
            var notificationResponse = new NotificationResponse();
            var notification = new Notification()
            {
                RequestType = RequestType.VendorOrder,
                RequestStatus = RequestStatus.Pending,
                RequestId = 234,
                RequestedBy = "Rajat Yadav",
                LastModified = DateTime.Now
            };
            var notificationList = new List<Notification>();
            notificationList.Add(notification);
            notificationResponse.Notifications = notificationList;
            notificationResponse.Status = Status.Success;
            return notificationResponse;
        }

    }
}
