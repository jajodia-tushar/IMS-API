using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ActivityLogsReponse : Response
    {
        public List<ActivityLogs> ActivityLogRecords { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
