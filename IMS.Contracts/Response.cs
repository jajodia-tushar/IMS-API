using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IMS.Contracts
{
    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }

        public Error Error { get; set; }

    }
}
