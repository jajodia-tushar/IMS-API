using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeOrder
    {
        public Employee Employee { get; set; }
        public EmployeeOrderDetails EmployeeOrderDetails { get; set; }
    }
}