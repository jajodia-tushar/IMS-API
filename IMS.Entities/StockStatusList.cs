using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatusList
    {
        public Item Item { get; set; }
        public List<StoreColourQuantity> StockStatus {get;set;}
    }
}
