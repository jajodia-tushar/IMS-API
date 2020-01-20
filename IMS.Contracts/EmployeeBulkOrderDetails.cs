using IMS.Contracts.Convertors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeBulkOrderDetails
    {
        public DateTime CreatedOn { get; set; }
        public DateTime RequirementDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BulkOrderRequestStatus BulkOrderRequestStatus { get; set; }
        public string ReasonForRequirement { get; set; }
        public List<ItemQuantityMapping> EmployeeItemsQuantityList { get; set; }
    }
}
