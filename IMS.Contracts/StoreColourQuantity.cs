using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Contracts
{
    public class StoreColourQuantity
    {
        string StoreName { get; set; }
        Colour Colour { get; set; }
        int Quantity { get; set; }
    }
}