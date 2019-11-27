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
        public EmployeeValidationResponse ValidateEmployee(EmployeeValidationRequest employeeValidationRequest)
        {
            EmployeeValidationResponse employeeValidationResponse = new EmployeeValidationResponse();
            try
            {
                if (employeeValidationRequest.Id <= 0)
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.MissingUsernameOrPassword
                    };
                    return employeeValidationResponse;
                }
                Employee employee = employeeDbContext.getEmployeeById(employeeValidationRequest.Id);
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
