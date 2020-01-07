using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationApi.Core.Models
{
    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }

        public Error Error { get; set; }

    }
}
