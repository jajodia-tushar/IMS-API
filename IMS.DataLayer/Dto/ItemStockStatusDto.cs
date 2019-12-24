using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class ItemStockStatusDto
    {
        public Dictionary<int, List<StockStatus>> StockStatus { get; set; }
        public List<Item> Items { get; set; }
    }
}
