using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    class EmployeeBulkOrderResponse: Response
    {
        public List<EmployeeBulkOrder> EmployeeBulkOrders { get; set; }
    }
}
