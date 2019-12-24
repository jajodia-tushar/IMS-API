using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Contracts
{
    public class ItemStockStatus
    {
        public int Quantity { get; set; }
        public string StoreName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Colour Colour { get; set; }

    }
}