using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeRecentOrderResponse : Response
    {
        public List<EmployeeRecentOrder> EmployeeRecentOrders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
