using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.EmployeeDataDumper
{
    public interface IEmployeesDataDbContext
    {
        List<Employee> CreateEmployee(List<Employee> employeesList);
    }
}
