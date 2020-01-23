using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace IMS.Entities
{
    public class EmployeeBulkOrderDetails
    {        
        public DateTime CreatedOn { get; set; }
        public DateTime RequirementDate { get; set; }
        public BulkOrderRequestStatus BulkOrderRequestStatus { get; set; }
        public string ReasonForRequirement { get; set; }
        public List<BulkOrderItemQuantityMapping> ItemsQuantityList { get; set; }
    }
}