using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatusResponse :Response
    {
        public List<string> NamesOfAllStores { get; set; }
        public List<StockStatusList> StockStatusList { get; set; }
    }
}
