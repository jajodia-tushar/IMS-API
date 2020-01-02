using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeDbContext employeeDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        public EmployeeService(IEmployeeDbContext employeeDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this.employeeDbContext = employeeDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<GetEmployeeResponse> ValidateEmployee(string employeeId)
        {
            GetEmployeeResponse employeeValidationResponse = new GetEmployeeResponse();
            try
            {
                if (String.IsNullOrEmpty(employeeId))
                {
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };
                    return employeeValidationResponse;
                }
                Employee employee = await employeeDbContext.GetEmployeeById(employeeId);
                if (employee != null)
                {
                    employeeValidationResponse.Status = Status.Success;
                    employeeValidationResponse.Employee = employee;
                }
                else
                {
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };
                }
            }
            catch (Exception exception)
            {
                employeeValidationResponse.Status = Status.Failure;
                employeeValidationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "ValidateEmployee", Severity.Critical, employeeId, employeeValidationResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeValidationResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeValidationResponse, "ValidateEmployee", employeeValidationResponse.Status, severity, -1); }).Start();
            }
            return employeeValidationResponse;
        }
    }
}
