using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemsConsumptionReport:Response
    {
        public List<DateItemConsumption> ItemConsumptions { get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}
