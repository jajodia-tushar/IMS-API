using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class VendorService : IVendorService
    {
        private IVendorDbContext _vendorDbContext;
        private ILogManager _logger;

        public VendorService(IVendorDbContext vendorDbContext, ILogManager logger)
        {
            this._vendorDbContext = vendorDbContext;
            this._logger = logger;

        }

        public async Task<VendorResponse> GetAllVendors()
        {
            VendorResponse getAllVendorsResponse = new VendorResponse();
            getAllVendorsResponse.Status = Status.Failure;
            getAllVendorsResponse.Error = new Error()
            {
                ErrorCode = Constants.ErrorCodes.NotFound,
                ErrorMessage = Constants.ErrorMessages.NoVendorsYet
            };
            try
            {
                List<Vendor> vendors = await _vendorDbContext.GetAllVendors();
                if (vendors != null)
                {
                    getAllVendorsResponse.Status = Status.Success;
                    getAllVendorsResponse.Vendors = vendors;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Severity severity = Severity.No;
                if (getAllVendorsResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("AllVendors", getAllVendorsResponse, "Getting all vendors", getAllVendorsResponse.Status, severity, -1); }).Start();
            }
            return getAllVendorsResponse;
        }

        public async Task<VendorResponse> GetVendorById(int vendorId)
        {
            VendorResponse getVendorResponse = new VendorResponse();
            getVendorResponse.Status = Status.Failure;
            getVendorResponse.Vendors = new List<Vendor>();
            getVendorResponse.Error = new Error()
            {
                ErrorCode = Constants.ErrorCodes.NotFound,
                ErrorMessage = Constants.ErrorMessages.InValidId
            };
            try
            {
                if (vendorId > 0)
                {
                    Vendor vendor = await _vendorDbContext.GetVendorById(vendorId);
                    if (vendor != null)
                    {
                        getVendorResponse.Status = Status.Success;
                        getVendorResponse.Vendors.Add(vendor);

                    }
                    return getVendorResponse;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Severity severity = Severity.No;
                if (getVendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendorId, getVendorResponse, "Getting Vendor", getVendorResponse.Status, severity, -1); }).Start();
            }
            return getVendorResponse;
        }
    }
}
