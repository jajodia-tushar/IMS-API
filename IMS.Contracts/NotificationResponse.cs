using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class NotificationResponse:Response
    {
        public List<Notification> Notifications { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
