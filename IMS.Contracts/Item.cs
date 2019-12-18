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
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public double Rate { get; set; }
        public int ShelvesRedLimit { get; set; }
        public int ShelvesAmberLimit { get; set; }
        public int WarehouseRedLimit { get; set; }
        public int WarehouseAmberLimit { get; set; }
    }
}