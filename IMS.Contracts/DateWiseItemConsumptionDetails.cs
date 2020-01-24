using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class DateWiseItemConsumptionDetails
    {
        public Item Item { get; set; }
        public List<DateItemConsumption> DateItemConsumptions { get; set; }
    }
}