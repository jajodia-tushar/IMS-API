using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Convertors
{
    public class BulkOrderRequestStatusConvertor : StringEnumConverter
    {
        BulkOrderRequestStatus defaultValue;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException)
            {
                return defaultValue;
            }
        }
    }
}
