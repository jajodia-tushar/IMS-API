using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemQuantityPriceMapping
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

    }
}
