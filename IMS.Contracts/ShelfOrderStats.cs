using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfOrderStats
    {
        public DateTime Date { get; set; }
        public List<ShelfOrderCountMapping> ShelfOrderCountMappings { get; set; }
    }
}
