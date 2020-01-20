using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeBulkOrdersResponse: Response
    {
        public List<EmployeeBulkOrder> EmployeeBulkOrders { get; set; }
    }
}
