using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class StockStatusResponse : Response
    {
        public List<StockStatusList> StockStatusList { get; set; }
    }
}
