using System;
using System.Collections.Generic;

namespace IMS.Entities
{
    public class EmployeeBulkOrderDetails
    {
        public int BulkOrderId { get; set; }
        public DateTime Date { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; }
        public string ReasonForRequirement { get; set; }
        public List<ItemQuantityMapping> EmployeeItemsQuantityList { get; set; }
    }
}