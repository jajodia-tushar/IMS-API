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

        // GET: api/Default/5
        [HttpGet("{id}", Name = "Get")]
        public EmployeeValidationResponse Get(int id)
        {
            EmployeeValidationResponse contractsEmployeeValidationResponse = null;
            try
            {
                IMS.Entities.EmployeeValidationResponse entityEmployeeValidationResponse = employeeService.ValidateEmployee(id);
                contractsEmployeeValidationResponse = Translator.ToDataContractsObject(entityEmployeeValidationResponse);
            }
            catch
            {
                contractsEmployeeValidationResponse = new IMS.Contracts.EmployeeValidationResponse()
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
