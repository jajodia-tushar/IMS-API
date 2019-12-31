using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeResponse : Response
    {
        public List<Employee> Employees { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
