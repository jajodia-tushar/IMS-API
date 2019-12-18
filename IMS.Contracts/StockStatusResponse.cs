using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class StockStatusResponse : Response
    {
        public List<string> NamesOfAllStores { get; set; }
        public List<StockStatusList> StockStatusList { get; set; }
    }
}
