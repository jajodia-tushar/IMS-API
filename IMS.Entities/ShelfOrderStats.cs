using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;

namespace IMS.Entities
{
    public class ShelfOrderStats
    {
        public DateTime Date { get; set; }
        public List<ShelfOrderCountMapping> ShelfOrderCountMappings { get; set; }
    }
}
