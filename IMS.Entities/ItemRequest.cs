using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemRequest
    {
        public Item item { get; set; }
        public List<ShelfMinimumLimitMapping> ShelfMinimumLimit { get; set; }
        public int WarehouseMinimumLimit { get; set; }

    }
}
