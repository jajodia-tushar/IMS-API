using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class DateWiseItemsConsumption : Response
    {
        public List<DateItemsMapping> DateItemMapping { get; set; }
    }
}
