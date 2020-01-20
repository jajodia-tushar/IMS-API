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
        Task<EmployeeResponse> GetAllEmployees(string filter, int limit, int offset);
        Task<string> CreateEmployee(Employee employee);
        Task<bool> CheckEmpEmailAvailability(string email);
        Task<bool> CheckEmployeeIdAvailability(string employeeId);
        Task<bool> Delete(string employeeId, bool isHardDelete);
        Task<Employee> Update(Employee updatedEmployee);
        Task<bool> EmployeeDetailsRepetitionCheck(Employee employee);
    }
}
