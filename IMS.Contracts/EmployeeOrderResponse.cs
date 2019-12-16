using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class EmployeeOrderResponse : Response
    {
        public EmployeeOrder EmployeeOrder { get; set; }
    }
}
