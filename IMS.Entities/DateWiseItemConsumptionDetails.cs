using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class DateWiseItemConsumptionDetails
    {
        public Item Item { get; set; }
        public List<DateItemConsumption> DateItemConsumptions { get; set; }
    }
}