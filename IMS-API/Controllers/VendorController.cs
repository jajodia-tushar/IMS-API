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
        [HttpGet("{id}", Name = "Get(int id)")]
        public GetVendorResponse GetVendorById(int id)
        {
            GetVendorResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.GetVendorResponse entityVendorValidationResponse = _vendorService.GetVendorById(id);
                contractsVendorValidationResponse = Translator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch
            {
                contractsVendorValidationResponse = new IMS.Contracts.GetVendorResponse()
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
        [HttpGet(Name = "Get()")]
        public GetAllVendorsResponse GetAllVendors()
        {
            GetAllVendorsResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.GetAllVendorsResponse entityVendorValidationResponse = _vendorService.GetAllVendors();
                contractsVendorValidationResponse = Translator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch
            {
                contractsVendorValidationResponse = new IMS.Contracts.GetAllVendorsResponse()
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
