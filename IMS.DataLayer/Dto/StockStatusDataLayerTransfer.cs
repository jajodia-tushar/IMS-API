using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class StockStatusDataLayerTransfer
    {
        public Dictionary<int, List<ItemStockStatus>> StockStatusDict { get; set; }
        public List<Item> ItemList { get; set; }
    }
}
