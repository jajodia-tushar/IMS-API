using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
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
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;

        public VendorService(IVendorDbContext vendorDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._vendorDbContext = vendorDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<VendorResponse> GetAllVendors()
        {
            VendorResponse getAllVendorsResponse = new VendorResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
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
                }
                else
                {
                    getAllVendorsResponse.Status = Status.Failure;
                    getAllVendorsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                getAllVendorsResponse.Status = Status.Failure;
                getAllVendorsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                new Task(() => { _logger.LogException(exception, "GetAllVendors", Severity.Medium, null, getAllVendorsResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (getAllVendorsResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("AllVendors", getAllVendorsResponse, "Getting all vendors", getAllVendorsResponse.Status, severity, userId); }).Start();
            }
            return getAllVendorsResponse;
        }

        public async Task<VendorResponse> GetVendorById(int vendorId)
        {
            VendorResponse getVendorResponse = new VendorResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
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
                }
                else
                {
                    getVendorResponse.Status = Status.Failure;
                    getVendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                getVendorResponse.Status = Status.Failure;
                getVendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                new Task(() => { _logger.LogException(exception, "GetVendorById", Severity.High, vendorId, getVendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (getVendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendorId, getVendorResponse, "Getting Vendor", getVendorResponse.Status, severity, userId); }).Start();
            }
            return getVendorResponse;
        }
    }
}
