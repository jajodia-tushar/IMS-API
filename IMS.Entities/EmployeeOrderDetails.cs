using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeOrderDetails
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public Shelf Shelf { get; set; }
        public List<ItemQuantityMapping> EmployeeItemsQuantityList { get; set; }
    }
}
