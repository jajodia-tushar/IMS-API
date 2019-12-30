using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeDbContext
    {
        Employee GetEmployeeById(string employeeId);
        List<Employee> GetAllEmployees();
        bool CreateEmployee(Employee employee);
    }
}
