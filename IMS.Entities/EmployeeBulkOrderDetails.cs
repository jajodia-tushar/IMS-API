using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace IMS.Entities
{
    public class EmployeeBulkOrderDetails
    {
        public int BulkOrderId { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequirementDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BulkOrderRequestStatus BulkOrderRequestStatus { get; set; }
        public string ReasonForRequirement { get; set; }
        public List<ItemQuantityMapping> EmployeeItemsQuantityList { get; set; }
    }
}