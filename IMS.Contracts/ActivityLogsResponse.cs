using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ActivityLogsResponse : Response
    {
        public List<ActivityLogs> ActivityLogRecords { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
