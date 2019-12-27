using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class LogsResponse:Response
    {
        public List<Logs> LogsRecords { get; set; }
    }
}
