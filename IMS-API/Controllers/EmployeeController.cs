using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EmployeeController : ControllerBase
    {
        private IEmployeeService employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
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
            catch
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
            }
            return contractsEmployeeValidationResponse;
        }
    }
}
