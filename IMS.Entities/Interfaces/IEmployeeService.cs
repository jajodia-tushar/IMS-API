using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        Task<GetEmployeeResponse> ValidateEmployee(string employeeId);
        Task<EmployeeResponse> GetAllEmployees(string filter,int pageNumber, int pageSize);
        Task<EmployeeResponse> Add(Employee employee);
        Task<EmployeeResponse> Update(Employee employee);
        Task<Response> Delete(string id,bool isHardDelete);

        Task<Response> CheckEmailAvailability(string email);
    }
}
