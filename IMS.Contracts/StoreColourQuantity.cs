﻿using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Contracts
{
    public class StoreColourQuantity
    {
        public string StoreName { get; set; }
        public Colour Colour { get; set; }
        public int Quantity { get; set; }
    }
}