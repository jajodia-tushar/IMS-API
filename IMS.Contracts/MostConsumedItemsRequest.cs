using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class MostConsumedItemsRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ItemsCount { get; set; }
    }
}
