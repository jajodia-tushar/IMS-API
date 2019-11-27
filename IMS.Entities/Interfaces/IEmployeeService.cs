using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        EmployeeValidationResponse ValidateEmployee(EmployeeValidationRequest employeeValidationRequest);
    }
}
