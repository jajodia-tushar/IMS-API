using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeOrderResponse : Response
    { 
            public List<EmployeeOrder> EmployeeOrders { get; set; }
            public PagingInfo PagingInfo { get; set; }
    }
}
