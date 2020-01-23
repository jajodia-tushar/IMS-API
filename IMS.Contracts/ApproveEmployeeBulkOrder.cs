using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ApproveEmployeeBulkOrder:EmployeeBulkOrder
    {
        List<ItemLocationQuantityMapping> ItemLocationQuantityMappings { get; set; }
    }
}
