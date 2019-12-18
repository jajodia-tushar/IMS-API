using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;

namespace IMS.Entities
{
    public class DateShelfOrderMapping
    {
        public DateTime Date;
        public List<ShelfOrderCountMapping> ShelfOrderCountMappings;
    }
}
