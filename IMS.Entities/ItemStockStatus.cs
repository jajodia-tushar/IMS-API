using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemStockStatus
    {
        public Item Item { get; set; }
        public List<StockStatus> StoreStatus {get;set;}
    }
}
