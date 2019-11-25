using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IMS.Entities
{
    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }
       
    }
}
