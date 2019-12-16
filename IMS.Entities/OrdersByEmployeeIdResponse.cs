using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class OrdersByEmployeeIdResponse : Response
    {
        public Employee Employee { get; set; }
        public List<EmployeeOrderDetails> EmployeeOrders { get; set; }
    }
}
