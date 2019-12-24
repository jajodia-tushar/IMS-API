using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class StockStatusDataDto
    {
        public Dictionary<int, List<ItemStockStatus>> StockStatus { get; set; }
        public List<Item> Items { get; set; }
    }
}
