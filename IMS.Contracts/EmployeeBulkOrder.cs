using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeBulkOrder
    {
        public int BulkOrderId { get; set; }
        public Employee Employee { get; set; }
        public EmployeeBulkOrderDetails EmployeeBulkOrderDetails { get; set; }
    }
}
