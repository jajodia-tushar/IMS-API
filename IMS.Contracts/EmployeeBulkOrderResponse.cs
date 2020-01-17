using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeBulkOrderResponse : Response
    {
        public List<EmployeeBulkOrder> EmployeeBulkOrders { get; set; }
    }
}
