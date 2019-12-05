using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxLimit { get; set; }
        public bool isActive { get; set; }
    }
}