using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class NotificationResponse:Response
    {
        public List<Notification> Notifications { get; set; }
    }
}
