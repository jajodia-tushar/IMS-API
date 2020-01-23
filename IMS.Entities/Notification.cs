using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Notification
    {
        public int RequestId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestType RequestType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus RequestStatus { get; set; }
        public string RequestedBy { get; set; }
        public DateTime LastModified { get; set; }
    }
}
