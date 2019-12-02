using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class GetEmployeeResponse : Response
    {
        public Employee Employee { get; set; }
    }
}
