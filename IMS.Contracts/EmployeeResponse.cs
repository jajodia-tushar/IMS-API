using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeResponse : Response
    {
        public List<Employee> Employees { get; set; }
    }
}
