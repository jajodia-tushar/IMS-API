using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class GetEmployeeByIdResponse : Response
    {
        public Employee Employee { get; set; }
    }
}
