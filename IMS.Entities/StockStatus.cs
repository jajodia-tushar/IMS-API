using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatus
    {
        public Item Item { get; set; }
        public List<StoreColourQuantity> StoreStatus {get;set;}
    }
}
