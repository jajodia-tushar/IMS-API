using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<GetEmployeeResponse> GetEmployeeById(String employeeId)
        {
            GetEmployeeResponse contractsEmployeeValidationResponse = null;
            try
            {
                IMS.Entities.GetEmployeeResponse entityEmployeeValidationResponse = await employeeService.ValidateEmployee(employeeId);
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

        /// <summary>
        /// Retrieve the employees
        /// </summary>
        /// <returns>Details of Employees</returns>
        /// <response code="200">Returns Employee Details of employees with status</response>
        // GET: api/
        [HttpGet]
        public async Task<EmployeeResponse> Get(String filter,int pageNumber,int pageSize)
        {
            EmployeeResponse contractsEmployeeValidationResponse = null;
            try
            {
                IMS.Entities.EmployeeResponse entityEmployeeValidationResponse = await employeeService.GetAllEmployees(filter,pageNumber,pageSize);
                contractsEmployeeValidationResponse = EmployeeTranslator.ToDataContractsObject(entityEmployeeValidationResponse);
            }
            catch (Exception exception)
            {
                contractsEmployeeValidationResponse = new IMS.Contracts.EmployeeResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetEmployees", IMS.Entities.Severity.Critical, filter, contractsEmployeeValidationResponse); }).Start();
            }
            return contractsEmployeeValidationResponse;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Email")]

        public async Task<Response> CheckEmailAlreadyExists(string email)
        {
            Response dtoValidEmailResponse = null;
            try
            {
                IMS.Entities.Response doValidEmailResponse = await employeeService.CheckEmailAvailability(email);
                dtoValidEmailResponse = Translator.ToDataContractsObject(doValidEmailResponse);
            }
            catch(Exception exception)
            {
                dtoValidEmailResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError

                    }
                };
                new Task(() => { _logger.LogException(exception, "CheckEmailAlreadyExists", IMS.Entities.Severity.Critical, email, dtoValidEmailResponse); }).Start();
            }
            return dtoValidEmailResponse;
        }
        /// <summary>
        /// Creates A New Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Returns newly created Employee Data</returns>
        /// <response code="200">Returns Employee if Employee is added successfully otherwise it returns null with status failure</response>
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<EmployeeResponse> Add([FromBody]  Employee employee)
        {
            EmployeeResponse contractEmployeeResponse = null;
            try
            {
                IMS.Entities.Employee entityEmployeeRequest = EmployeeTranslator.ToEntitiesObject(employee);
                IMS.Entities.EmployeeResponse entityEmployeeResponse = await employeeService.Add(entityEmployeeRequest);
                contractEmployeeResponse = EmployeeTranslator.ToDataContractsObject(entityEmployeeResponse);
                contractEmployeeResponse.PagingInfo = null;
            }
            catch (Exception exception)
            {
                contractEmployeeResponse = new IMS.Contracts.EmployeeResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Add", IMS.Entities.Severity.High, employee, contractEmployeeResponse); }).Start();
            }
            return contractEmployeeResponse;
        }

        /// <summary>
        /// Update the Specific Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Returns updated employee data</returns>
        /// <response code="200">Returns updated Employee data if employee is updated successfully otherwise it returns null with status failure</response>
        [HttpPut]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<EmployeeResponse> Update([FromBody] Employee employee)
        {
            EmployeeResponse contractEmployeeResponse = null;
            try
            {
                IMS.Entities.Employee entityEmployeeRequest = EmployeeTranslator.ToEntitiesObject(employee);
                IMS.Entities.EmployeeResponse entityEmployeeResponse = await employeeService.Update(entityEmployeeRequest);
                contractEmployeeResponse = EmployeeTranslator.ToDataContractsObject(entityEmployeeResponse);
                contractEmployeeResponse.PagingInfo = null;
            }
            catch (Exception exception)
            {
                contractEmployeeResponse = new IMS.Contracts.EmployeeResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Update", IMS.Entities.Severity.High, employee, contractEmployeeResponse); }).Start();
            }
            return contractEmployeeResponse;
        }

        /// <summary>
        /// SoftDelete or HardDelete the specific Employee
        /// </summary>
        /// <param name="id">Id of that employee</param>
        /// <param name="isHardDelete">Boolean value for hard delete or soft delete</param>
        /// <response code="200">Returns Succeess if that employee is deleted successfully otherwise it returns null with status failure</response>
        [HttpDelete]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<Response> Delete(string id, bool isHardDelete)
        {
            Response contractEmployeeResponse = null;
            try
            {
                IMS.Entities.Response entityEmployeeResponse = await employeeService.Delete(id, isHardDelete);
                contractEmployeeResponse = Translator.ToDataContractsObject(entityEmployeeResponse);
            }
            catch (Exception exception)
            {
                contractEmployeeResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Delete", IMS.Entities.Severity.High, id + ";" + isHardDelete, contractEmployeeResponse); }).Start();
            }
            return contractEmployeeResponse;
        }
    }
}
