using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Entities
{
    public class StoreColourQuantity
    {
        string StoreName { get; set; }
        Colour Colour { get; set; }
        int Quantity { get; set; }
    }
}