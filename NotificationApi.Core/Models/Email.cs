using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationApi.Core.Models
{
    public class Email
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
    }
}
