using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeController : ControllerBase
    {
        private IEmployeeService employeeService;
        private ILogManager _logger;
        public EmployeeController(IEmployeeService employeeService, ILogManager logManager)
        {
            this.employeeService = employeeService;
            this._logger = logManager;
        }

        /// <summary>
        /// Retrieve the employee by their ID
        /// </summary>
        /// <param name="employeeId">The ID of the desired Employee</param>
        /// <returns>Details of Employee</returns>
        /// <response code="200">Returns Employee Details if employee id is valid otherwise it returns null with status failure</response>
        // GET: api/Default/5
        [HttpGet("{employeeId}", Name = "GetEmployeeById")]
        public GetEmployeeResponse GetEmployeeById(String employeeId)
        {
            GetEmployeeResponse contractsEmployeeValidationResponse = null;
            try
            {
                IMS.Entities.GetEmployeeResponse entityEmployeeValidationResponse = employeeService.ValidateEmployee(employeeId);
                contractsEmployeeValidationResponse = EmployeeTranslator.ToDataContractsObject(entityEmployeeValidationResponse);
            }
            catch (Exception exception)
            {
                contractsEmployeeValidationResponse = new IMS.Contracts.GetEmployeeResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetEmployeeById", IMS.Entities.Severity.Critical, employeeId, contractsEmployeeValidationResponse); }).Start();
            }
            return contractsEmployeeValidationResponse;
        }


        [HttpPost("Add")]
        public EmployeeResponse Add(string filePath)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            try
            {
                IMS.Entities.EmployeeResponse entityEmployeeResponse = employeeService.AddEmployeesFromCsvFile(filePath);
                employeeResponse = EmployeeTranslator.ToDataContractsObject(entityEmployeeResponse);
            }
            catch (Exception exception)
            {
                employeeResponse = new IMS.Contracts.EmployeeResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Add", IMS.Entities.Severity.High, filePath, employeeResponse); }).Start();
            }
            return employeeResponse;
        }
    }
}
