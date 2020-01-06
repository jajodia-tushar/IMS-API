using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Entities
{
    public class StockStatus
    {
        public string Location { get; set; }
        public Colour Colour { get; set; }
        public int Quantity { get; set; }
    }
}