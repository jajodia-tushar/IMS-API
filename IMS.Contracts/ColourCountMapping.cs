using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ColourCountMapping
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Colour Colour;
        public int Count;
    }
}
