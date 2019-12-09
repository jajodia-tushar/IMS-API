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
    public class VendorController : ControllerBase
    {
        private IVendorService _vendorService;
        public VendorController(IVendorService vendorService)
        {
            this._vendorService = vendorService;
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
                contractsVendorValidationResponse = Translator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch
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
                contractsVendorValidationResponse = Translator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch
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
            }
            return contractsVendorValidationResponse;
        }
    }
}
