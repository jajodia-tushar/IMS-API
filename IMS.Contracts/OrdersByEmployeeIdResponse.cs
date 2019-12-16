using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class OrdersByEmployeeIdResponse : Response
    {
        public Employee Employee { get; set; }

        public List<EmployeeOrderDetails> EmployeeOrders { get; set; }
    }
}
