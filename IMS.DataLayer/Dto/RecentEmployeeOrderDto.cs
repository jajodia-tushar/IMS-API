using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class RecentEmployeeOrderDto
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string TemporaryCardNumber { get; set; }
        public string AccessCardNumber { get; set; }
        public bool EmployeeStatus { get; set; }

        public int OrderId { get; set; }
        public DateTime EmployeeOrderDate { get; set; }
        public int ShelfId { get; set; }
        public string ShelfName { get; set; }
        public bool ShelfStatus { get; set; }
        public string ShelfCode { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public bool ItemStatus { get; set; }
        public int ItemQuantity { get; set; }

    }
}
