using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ApproveEmployeeBulkOrder : EmployeeBulkOrder
    {
        List<ItemLocationQuantityMapping> ItemLocationQuantityMappings { get; set; }
    }
}
