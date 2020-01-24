using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemConsumptionDetailsResponse : Response
    {
        public List<DateWiseItemConsumptionDetails> DateWiseItemConsumptionDetails { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
