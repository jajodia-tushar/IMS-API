using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Employee
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string TemporaryCardNumber { get; set; }
        public string AccessCardNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
