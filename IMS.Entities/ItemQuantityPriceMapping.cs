using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemQuantityPriceMapping
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice
        {
            get
            {
                double totalPrice = 0;
                if (this.Item != null)
                    totalPrice = this.Item.Rate * this.Quantity;
                return Math.Round(totalPrice, 4);
            }
        }
    }
}
