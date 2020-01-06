using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Email
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
    }
}
