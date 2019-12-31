﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        Task<GetEmployeeResponse> ValidateEmployee(string employeeId);
        GetEmployeeResponse ValidateEmployee(string employeeId);
        Task<EmployeeResponse> GetAllEmployees();
        Task<EmployeeResponse> GetEmployee(int employeeId);
        Task<EmployeeResponse> Add(Employee employee);
        Task<EmployeeResponse> Update(Employee employee);
        Task<EmployeeResponse> Delete(int id);
    }
}
