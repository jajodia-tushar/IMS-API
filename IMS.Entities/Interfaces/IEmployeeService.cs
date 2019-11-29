using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        EmployeeValidationResponse ValidateEmployee(int id);
    }
}
