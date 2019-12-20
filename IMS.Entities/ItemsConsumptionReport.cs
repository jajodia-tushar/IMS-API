using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemsConsumptionReport:Response
    {
        public List<DateItemConsumption> ItemConsumptions { get; set; }
    }
}