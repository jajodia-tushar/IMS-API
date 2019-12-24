using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class StockStatusResponse : Response
    {
        public List<ItemStockStatus> StockStatusList { get; set; }
    }
}
