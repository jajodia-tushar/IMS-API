using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeOrderDetails
    {
        public int OrderId { get; set; } 
        public DateTime Date { get; set; }
        public Shelf Shelf{ get; set; }
        public List<BulkOrderItemQuantityMapping> ItemsQuantityList { get; set; }
    }
}
