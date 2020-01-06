using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemsAvailabilityResponse : Response
    {
        public List<ItemQuantityMapping> ItemQuantityMappings;
        public PagingInfo pagingInfo { get; set; }
    }
}
