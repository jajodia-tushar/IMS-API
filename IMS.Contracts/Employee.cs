using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class Employee
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string TCardNumber { get; set; }
        public string AccessCardNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
