using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatusResponse :Response
    {
       public List<ItemStockStatus> StockStatusList { get; set; }
    }
}
