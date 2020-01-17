using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
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
