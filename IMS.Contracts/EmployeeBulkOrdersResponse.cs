using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeBulkOrdersResponse : Response
    {
        public List<EmployeeBulkOrder> EmployeeBulkOrders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
