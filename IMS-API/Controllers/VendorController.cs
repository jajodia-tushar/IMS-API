using System;
using System.Collections.Generic;
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
        public async Task<VendorsResponse> GetVendorById(int id)
        {
            VendorsResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.VendorsResponse entityVendorValidationResponse =await _vendorService.GetVendorById(id);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch(Exception exception)
            {
                contractsVendorValidationResponse = new IMS.Contracts.VendorsResponse()
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
        public async Task<VendorsResponse> Update([FromBody]Vendor vendor)
        {
            VendorsResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.Vendor vendorEntity = VendorTranslator.ToEntitiesObject(vendor);
                IMS.Entities.VendorsResponse entityVendorValidationResponse = await _vendorService.UpdateVendor(vendorEntity);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch (Exception exception)
            {
                contractsVendorValidationResponse = new IMS.Contracts.VendorsResponse()
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
        public async Task<VendorsResponse> Add([FromBody]Vendor vendor)
        {
            VendorsResponse contractsVendorValidationResponse = null;
            try
            {
                IMS.Entities.Vendor vendorEntity = VendorTranslator.ToEntitiesObject(vendor);
                IMS.Entities.VendorsResponse entityVendorValidationResponse = await _vendorService.AddVendor(vendorEntity);
                contractsVendorValidationResponse = VendorTranslator.ToDataContractsObject(entityVendorValidationResponse);
            }
            catch (Exception exception)
            {
                contractsVendorValidationResponse = new IMS.Contracts.VendorsResponse()
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
        // DELETE: api/
        /// <summary>
        /// returns delete status
        /// </summary>
        /// <param name="vendorId">Takes the id of the vendor to be deleted</param>
        /// <param name="isHardDelete">Values 1 or 0 corresponds to whether the deletion is a hard delete or a soft delete</param>
        /// <returns>deletion status</returns>
        /// <response code="200">deletion status</response>
        [HttpDelete("{vendorId}", Name = "Delete(int vendorId")]
        public async Task<Response> Delete(int vendorId, bool isHardDelete)
        {
            Response deleteVendor = null;
            try
            {
                IMS.Entities.Response deleteVendorEntity = await _vendorService.DeleteVendor(vendorId, isHardDelete);
                deleteVendor = Translator.ToDataContractsObject(deleteVendorEntity);
            }
            catch (Exception exception)
            {
                deleteVendor = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "DeleteVendor", IMS.Entities.Severity.Medium, vendorId, deleteVendor); }).Start();
            }
            return deleteVendor;
        }
        // GET: api/
        /// <summary>
        /// Check vendor detail uniqueness 
        /// </summary>
        /// <param name="name">takes the name to be checked for uniqueness</param>
        /// <param name="pan">takes the pan to be checked for uniqueness</param>
        /// <param name="cin">takes the cin to be checked for uniqueness</param>
        /// <param name="mobile">takes the mobile to be checked for uniqueness</param>
        /// <param name="gst">takes the gst to be checked for uniqueness</param>
        /// <returns>the response of uniqueness vendor</returns>
        [HttpGet("IsUnique",Name = "IsCheckUnique(string name, string pan, string gst, string mobile,string cin)")]
        public async Task<Response> IsCheckUnique(string name, string pan, string gst, string mobile,string cin)
        {
            Response contractCheckDetailUniqueness = null;
            try
            {
                IMS.Entities.Response entityVendorUniquenessResponse = await _vendorService.CheckUniqueness(name,pan,gst,mobile,cin);
                contractCheckDetailUniqueness = Translator.ToDataContractsObject(entityVendorUniquenessResponse);
            }
            catch (Exception exception)
            {
                contractCheckDetailUniqueness = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Get", IMS.Entities.Severity.High, "checking uniqueness", contractCheckDetailUniqueness); }).Start();
            }
            return contractCheckDetailUniqueness;
        }
    }
}
