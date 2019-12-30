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
        /// returns all vendors if name is null or returns vendors with name matching the name provided in request
        /// </summary>
        /// <returns>the vendors object along with status</returns>
        /// <response code="200">Returns the Vendors object</response>
        [HttpGet(Name = "Get(string Name)")]
        public async Task<VendorsResponse> Get(string name, int pageNumber, int pageSize )
        {

            VendorsResponse vendorResponse = null;
            try
            {
                IMS.Entities.VendorsResponse vendorResponseEntity = await _vendorService.GetVendors(name,pageNumber, pageSize);
                vendorResponse = VendorTranslator.ToDataContractsObject(vendorResponseEntity);
            }
            catch (Exception exception)
            {
                vendorResponse = new VendorsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetVendors", IMS.Entities.Severity.High, name, vendorResponse); }).Start();
            }
            return vendorResponse;
        }
        // PUT: api/
        /// <summary>
        /// updates vendor details
        /// </summary>
        /// <param name="vendor">takes the vendor to be updated</param>
        /// <returns>the updated vendor</returns>
        /// <response code="200">Returns updated vendor</response>
        [HttpPut(Name = "Update(Vendor vendor)")]
        public async Task<VendorResponse> Update([FromBody]Vendor vendor)
        {
            VendorResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.Vendor vendorEntity = VendorTranslator.ToEntitiesObject(vendor);
                IMS.Entities.VendorResponse entityVendorValidationResponse = await _vendorService.UpdateVendor(vendorEntity);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch (Exception exception)
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
                new Task(() => { _logger.LogException(exception, "Update", IMS.Entities.Severity.High, vendor, contractsVendorValidationResponse); }).Start();
            }
            return contractsVendorValidationResponse;
        }
        // POST: api/
        /// <summary>
        /// Add vendor 
        /// </summary>
        /// <param name="vendor">takes the vendor to be added</param>
        /// <returns>the added vendor</returns>
        /// <response code="200">Returns the added vendor</response>
        [HttpPost(Name = "Add(Vendor vendor)")]
        public async Task<VendorResponse> Add([FromBody]Vendor vendor)
        {
            VendorResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.Vendor vendorEntity = VendorTranslator.ToEntitiesObject(vendor);
                IMS.Entities.VendorResponse entityVendorValidationResponse = await _vendorService.AddVendor(vendorEntity);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch (Exception exception)
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
                new Task(() => { _logger.LogException(exception, "Add", IMS.Entities.Severity.High, vendor, contractsVendorValidationResponse); }).Start();
            }
            return contractsVendorValidationResponse;
        }
    }
}
