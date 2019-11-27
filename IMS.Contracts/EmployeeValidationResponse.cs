using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeValidationResponse : Response
    {
        public Employee Employee { get; set; }
    }
}
