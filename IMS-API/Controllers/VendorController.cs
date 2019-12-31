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
    public class VendorController : ControllerBase
    {
        private IVendorService _vendorService;
        private ILogManager _logger;
        public VendorController(IVendorService vendorService, ILogManager logManager)
        {
            this._vendorService = vendorService;
            this._logger = logManager;
        }

        // GET: api/Default/5
        /// <summary>
        /// returns vendor object if id is present, otherwise failure
        /// </summary>
        /// <param name="id">Here vendor id is used to identify the vendor</param>
        /// <returns>entire vendor object along with status</returns>
        /// <response code="200">Returns VendorOrder object  if Vendor id is valid otherwise it returns null and status failure</response>
        [HttpGet("{id}", Name = "Get(int id)")]
        public async Task<VendorResponse> GetVendorById(int id)
        {
            VendorResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.VendorResponse entityVendorValidationResponse =await _vendorService.GetVendorById(id);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch(Exception exception)
            {
                contractsVendorValidationResponse = new IMS.Contracts.VendorResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetVendorById", IMS.Entities.Severity.Medium, id, contractsVendorValidationResponse); }).Start();
            }
            return contractsVendorValidationResponse;
        }
        // GET: api/
        /// <summary>
        /// returns all vendors 
        /// </summary>
        /// <returns>all vendors object along with status</returns>
        /// <response code="200">Returns all Vendors object</response>
        [HttpGet(Name = "Get()")]
        public async Task<VendorResponse> GetAllVendors()
        {
            VendorResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.VendorResponse entityVendorValidationResponse =await _vendorService.GetAllVendors();
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch(Exception exception)
            {
                contractsVendorValidationResponse = new IMS.Contracts.VendorResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetAllVendors", IMS.Entities.Severity.High, null, contractsVendorValidationResponse); }).Start();
            }
            return contractsVendorValidationResponse;
        }

        /// <summary>
        /// Returns recent order placed by the employee with employee and employee order details
        /// </summary>
        /// <returns>List of employee recent order along with the status</returns>
        /// <response code="200">Returns the employee recent order along with status success </response>
        /// <response code="400">If Unable to show recent entries </response>
        /// <response code="401">If token is Invalid</response>
        /// <response code="403">If Username and Password credentials are not of Admin and SuperAdmin</response>
        // GET: api/Order/EmployeeRecentOrderDetails
        [HttpGet("SearchByName/{name}", Name = "SearchByName(string Name)")]
        public async Task<VendorSearchResponse> SearchByName(string name, int pageNumber, int pageSize )
        {

            VendorSearchResponse vendorResponse = null;
            try
            {
                IMS.Entities.VendorSearchResponse vendorResponseEntity = await _vendorService.SearchByName(name,pageNumber, pageSize);
                vendorResponse = VendorTranslator.ToDataContractsObject(vendorResponseEntity);
            }
            catch (Exception exception)
            {
                vendorResponse = new VendorSearchResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "SearchvendorByName", IMS.Entities.Severity.High, name, vendorResponse); }).Start();
            }
            return vendorResponse;
        }
    }
}
