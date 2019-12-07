using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeDbContext employeeDbContext;
        private ILogManager _logger;
        public EmployeeService(IEmployeeDbContext employeeDbContext, ILogManager logger)
        {
            this.employeeDbContext = employeeDbContext;
            this._logger = logger;
        }
        public GetEmployeeResponse ValidateEmployee(string employeeId)
        {
            GetEmployeeResponse employeeValidationResponse = new GetEmployeeResponse();
            try
            {
                if (String.IsNullOrEmpty(employeeId))
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };
                    return employeeValidationResponse;
                }
                Employee employee = employeeDbContext.GetEmployeeById(employeeId);
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
            finally
            {
                Severity severity = Severity.No;
                if (employeeValidationResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeValidationResponse, "Validating employee", employeeValidationResponse.Status, severity, -1); }).Start();
            }
            return employeeValidationResponse;
        }

    }
}
