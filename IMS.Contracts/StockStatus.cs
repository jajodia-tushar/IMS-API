using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
namespace IMS.Contracts
{
    public class StockStatus
    {
        public int Quantity { get; set; }
        public string Location { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Colour Colour { get; set; }

    }
}