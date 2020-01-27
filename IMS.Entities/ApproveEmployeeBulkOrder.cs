using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ApproveEmployeeBulkOrder : EmployeeBulkOrder
    {
        public List<ItemLocationQuantityMapping> ItemLocationQuantityMappings { get; set; }
    }
}
