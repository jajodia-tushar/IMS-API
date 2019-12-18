using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatusResponse :Response
    {
        public Dictionary<Item,List<StoreColourQuantity>> StockStatus { get; set; }
    }
}
