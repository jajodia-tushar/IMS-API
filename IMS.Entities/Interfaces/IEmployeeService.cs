using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        Task<GetEmployeeResponse> ValidateEmployee(string employeeId);
        GetEmployeeResponse ValidateEmployee(string employeeId);
        Task<EmployeeResponse> GetAllEmployees(int pageNumber, int pageSize);
        Task<GetEmployeeResponse> GetEmployee(int employeeId);
        Task<GetEmployeeResponse> Add(Employee employee);
        Task<GetEmployeeResponse> Update(Employee employee);
        Task<Response> Delete(int id);
    }
}
