using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class GetEmployeeResponse :Response
    {
        public Employee Employee { get; set; }
    }
}
