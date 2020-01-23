using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class BulkOrderItemQuantityMapping
    {
        public Item Item { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityUsed { get; set; }
    }
}
