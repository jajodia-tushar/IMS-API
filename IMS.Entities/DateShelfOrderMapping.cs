using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;

namespace IMS.Entities
{
    public class DateShelfOrderMapping
    {
        DateTime Date { get; set; }
        List<ShelfOrderCountMapping> ShelfOrderCountMappings { get; set; }
    }
}
