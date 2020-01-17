using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    class EmployeeBulkOrder
    {
        public Employee Employee { get; set; }
        public EmployeeBulkOrderDetails EmployeeBulkOrderDetails { get; set; }
    }
}
