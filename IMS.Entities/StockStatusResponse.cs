using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class StockStatusResponse :Response
    {
       public List<StockStatus> StockStatusList { get; set; }
    }
}
