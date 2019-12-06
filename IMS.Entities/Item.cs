using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxLimit { get; set; }
        public bool IsActive { get; set; }
    }
}
