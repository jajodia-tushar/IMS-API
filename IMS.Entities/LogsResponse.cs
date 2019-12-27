using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class LogsResponse:Response
    {   
        public List<Logs> LogsRecords { get; set; }
    }
}
