using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfOrderCountMapping
    {
        public string ShelfName { get; set; }
        public int ?OrderCount { get; set; }
    }
}
