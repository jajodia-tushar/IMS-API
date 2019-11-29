using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class GetEmployeeByIdResponse :Response
    {
        public Employee Employee { get; set; }
    }
}
