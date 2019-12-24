using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatus
    {
        public Item Item { get; set; }
        public List<ItemStockStatus> StoreStatus {get;set;}
    }
}
