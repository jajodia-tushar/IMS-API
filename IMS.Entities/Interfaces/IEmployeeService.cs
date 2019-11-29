using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        GetEmployeeByIdResponse ValidateEmployee(int id);
    }
}
