using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeDbContext
    {
        Task<Employee> GetEmployeeById(string employeeId);
        Task<List<Employee>> GetAllEmployees();
        Task<bool> CreateEmployee(Employee employee);

        Task<bool> CheckEmpEmailAvailability(string email);
    }
}
