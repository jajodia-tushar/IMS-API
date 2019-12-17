using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class MostConsumedItemsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ItemsCount { get; set; }
    }
}
