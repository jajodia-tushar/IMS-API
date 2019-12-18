using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class StockStatusResponse : Response
    {
        public Dictionary<Item, List<StoreColourQuantity>> StockStatus { get; set; }
    }
}
