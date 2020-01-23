using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ActivityLogs
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string PerformedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Remarks { get; set; }
    }
}
