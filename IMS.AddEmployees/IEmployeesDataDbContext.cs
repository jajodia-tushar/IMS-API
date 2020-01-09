using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.EmployeeDataDumper
{
    public interface IEmployeesDataDbContext
    {
        Task<List<Employee>> CreateEmployee(List<Employee> employeesList);
    }
}
