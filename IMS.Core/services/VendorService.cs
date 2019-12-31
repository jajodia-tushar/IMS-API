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
            VendorResponse vendorResponse = new VendorResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        List<Vendor> vendors = await _vendorDbContext.GetAllVendors();
                        if (vendors.Count != 0)
                        {
                            vendorResponse.Status = Status.Success;
                            vendorResponse.Vendors = vendors;
                        }
                        else
                        {
                            vendorResponse.Status = Status.Failure;
                            vendorResponse.Error = new Error()
                            {
                                ErrorCode = Constants.ErrorCodes.NotFound,
                                ErrorMessage = Constants.ErrorMessages.NoVendorsYet
                            };
                        }
                    }
                    catch (Exception exception)
                    {
                        vendorResponse.Status = Status.Failure;
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                        new Task(() => { _logger.LogException(exception, "GetAllVendors", Severity.Medium, null, vendorResponse); }).Start();
                    }
                }
                else
                {
                    vendorResponse.Status = Status.Failure;
                    vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                vendorResponse.Status = Status.Failure;
                vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                new Task(() => { _logger.LogException(exception, "GetAllVendors", Severity.Medium, null, vendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("AllVendors", vendorResponse, "Getting all vendors", vendorResponse.Status, severity, userId); }).Start();
            }
            return vendorResponse;
        }

        public async Task<VendorResponse> GetVendorById(int vendorId)
        {
            VendorResponse vendorResponse = new VendorResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    vendorResponse.Vendors = new List<Vendor>();
                    userId = user.Id;
                    try
                    {
                        if (vendorId > 0)
                        {
                            Vendor vendor = await _vendorDbContext.GetVendorById(vendorId);
                            if (vendor != null)
                            {
                                vendorResponse.Status = Status.Success;
                                vendorResponse.Vendors.Add(vendor);

                            }
                            return vendorResponse;
                        }
                        else
                        {
                            vendorResponse.Status = Status.Failure;
                            vendorResponse.Error = new Error()
                            {
                                ErrorCode = Constants.ErrorCodes.NotFound,
                                ErrorMessage = Constants.ErrorMessages.InValidId
                            };
                        }
                    }
                    catch (Exception exception)
                    {
                        vendorResponse.Status = Status.Failure;
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                        new Task(() => { _logger.LogException(exception, "GetAllVendors", Severity.Medium, null, vendorResponse); }).Start();
                    }
                }
                else
                {
                    vendorResponse.Status = Status.Failure;
                    vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                vendorResponse.Status = Status.Failure;
                vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.LogoutFailed);
                new Task(() => { _logger.LogException(exception, "GetVendorById", Severity.High, vendorId, vendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendorId, vendorResponse, "Getting Vendor", vendorResponse.Status, severity, userId); }).Start();
            }
            return vendorResponse;
        }

        public async Task<VendorSearchResponse> SearchByName(string name, int pageNumber, int pageSize)
        {
            VendorSearchResponse vendorResponse = new VendorSearchResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    if (pageSize == 0)
                        pageSize = 10;
                    if (pageNumber == 0)
                        pageNumber = 1;
                    vendorResponse.PagingInfo = new PagingInfo()
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    List<Vendor> vendors = new List<Vendor>();
                    vendorResponse.PagingInfo.TotalResults = await _vendorDbContext.SearchByName(name, limit, offset, vendors);
                    if (vendors == null || vendors.Count == 0)
                    {
                        vendorResponse.Status = Status.Failure;
                        vendorResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.NoVendorsYet
                        };
                    }
                    else
                    {
                        vendorResponse.Status = Status.Success;
                        vendorResponse.Vendors = vendors;
                    }
                }
                else
                {
                    vendorResponse.Status = Status.Failure;
                    vendorResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.UnAuthorized,
                        ErrorMessage = Constants.ErrorMessages.InvalidToken
                    };
                }
            }
            catch (Exception exception)
            {
                vendorResponse.Error.ErrorCode = Constants.ErrorCodes.BadRequest;
                vendorResponse.Error.ErrorMessage = Constants.ErrorMessages.UnableToFetch;
            }
            finally
            {
                Severity severity = Severity.Medium;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("SearchByName", vendorResponse, "SearchByName", vendorResponse.Status, severity, -1); }).Start();
            }
            return vendorResponse;

        }
    }
}
