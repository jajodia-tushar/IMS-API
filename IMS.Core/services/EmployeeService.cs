using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeDbContext employeeDbContext;
        public EmployeeService(IEmployeeDbContext employeeDbContext)
        {
            this.employeeDbContext = employeeDbContext;
        }
        public EmployeeValidationResponse ValidateEmployee(int id)
        {
            EmployeeValidationResponse employeeValidationResponse = new EmployeeValidationResponse();
            try
            {
                if (id <= 0)
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };
                    return employeeValidationResponse;
                }
                Employee employee = employeeDbContext.getEmployeeById(id);
                if (employee != null)
                {
                    employeeValidationResponse.Status = Status.Success;
                    employeeValidationResponse.Employee = employee;

                }
                else
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return employeeValidationResponse;
        }

    }
}
