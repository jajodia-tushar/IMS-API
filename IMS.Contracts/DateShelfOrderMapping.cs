using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class DateShelfOrderMapping
    {
        DateTime Date { get; set; }
        List<ShelfOrderCountMapping> ShelfOrderCountMappings { get; set; }
    }
}
