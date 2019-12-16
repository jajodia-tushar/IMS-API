using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeRecentOrder
    {
        public Employee Employee { get; set; }
        public EmployeeOrderDetails EmployeeOrder { get; set; }
    }
}
