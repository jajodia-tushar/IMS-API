using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    class EmployeeBulkOrderDto
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string TemporaryCardNumber { get; set; }
        public string AccessCardNumber { get; set; }
        public bool EmployeeStatus { get; set; }

        public int BulkOrderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime RequirementDate { get; set; }
        public string RequestStatus { get; set; }
        public string ReasonForRequirement { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public bool ItemStatus { get; set; }
        public int ItemQuantityOrdered { get; set; }
        public int ItemQuantityUsed { get; set; }
        public bool ItemIsActive { get; set; }
        public int ItemMaxLimit { get; set; }
        public string ItemImageUrl { get; set; }
    }
}
