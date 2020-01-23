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
            var notification1 = new Notification()
            {
                RequestType = RequestType.VendorOrder,
                RequestStatus = RequestStatus.Edited,
                RequestId = 234,
                RequestedBy = "Rajat Yadav",
                LastModified = DateTime.Now
            };
            var notification2 = new Notification()
            {
                RequestType = RequestType.BulkOrder,
                RequestStatus = RequestStatus.Pending,
                RequestId = 267,
                RequestedBy = "Ebran Khan",
                LastModified = DateTime.Now
            };
            var notification3 = new Notification()
            {
                RequestType = RequestType.UserModification,
                RequestStatus = RequestStatus.Approved,
                RequestId = 132,
                RequestedBy = "Aniket Singla",
                LastModified = DateTime.Now
            };
            var notificationList = new List<Notification>();
            notificationList.Add(notification1);
            notificationList.Add(notification2);
            notificationList.Add(notification3);
            notificationResponse.Notifications = notificationList;
            notificationResponse.Status = Status.Success;
            return notificationResponse;
        }

    }
}
