using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeBulkOrderDetails
    {
        public DateTime Date { get; set; }
        public DateTime RequirementDate { get; set; }
        public BulkOrderRequestStatus BulkOrderRequestStatus { get; set; }
        public string ReasonForRequirement { get; set; }
        public List<ItemQuantityMapping> EmployeeItemsQuantityList { get; set; }
    }
}
