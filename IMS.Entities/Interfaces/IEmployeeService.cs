using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        GetEmployeeResponse ValidateEmployee(string employeeId);
    }
}
