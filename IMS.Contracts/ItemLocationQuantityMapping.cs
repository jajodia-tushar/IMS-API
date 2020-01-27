using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemLocationQuantityMapping
    {
        public Item Item { get; set; }
        public List<LocationQuantityMapping> LocationQuantityMappings { get; set; }
    }
}
