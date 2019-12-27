using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Logs
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string CallType { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
