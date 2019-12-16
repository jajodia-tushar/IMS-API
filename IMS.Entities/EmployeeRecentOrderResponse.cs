using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeRecentOrderResponse : Response
    {
        public List<EmployeeRecentOrder> EmployeeRecentOrders { get; set; }
    }
}
