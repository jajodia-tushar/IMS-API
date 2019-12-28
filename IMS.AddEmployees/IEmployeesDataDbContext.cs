using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.EmployeeDataDumper
{
    public interface IEmployeesDataDbContext
    {
        List<String> CreateEmployee(List<Employee> employeesList);
    }
}
