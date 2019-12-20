using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemsConsumptionCountResponse:Response
    {
        public List<DateItemConsumption> DateItemConsumptionList { get; set; }
    }
}
