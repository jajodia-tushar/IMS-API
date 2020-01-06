using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationApi.Core.Models
{
    public class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
