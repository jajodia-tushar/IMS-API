using IMS.Core.Validators;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
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
        [Audit("Added Vendor","Vendor")]
        public async Task<VendorsResponse> AddVendor(Vendor vendor)
        {
            VendorsResponse vendorResponse = new VendorsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    vendorResponse.Vendors = new List<Vendor>();
                    if (VendorValidator.Validate(vendor))
                    {
                        if (await _vendorDbContext.IsVendorPresent(vendor))
                        {
                            vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnprocessableEntity, Constants.ErrorMessages.DataAlreadyPresent);
                        }
                        else
                        {
                            vendor = await _vendorDbContext.AddVendor(vendor);
                            if (vendor != null)
                            {
                                vendorResponse.Status = Status.Success;
                                vendorResponse.Vendors.Add(vendor);
                            }
                        }
                        return vendorResponse;
                    }
                    else
                    {
                        vendorResponse.Status = Status.Failure;
                        vendorResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.MissingValues
                        };
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
                vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "AddVendor", Severity.High, vendor, vendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendor, vendorResponse, "AddVendor", vendorResponse.Status, severity, userId); }).Start();
            }
            return vendorResponse;
        }

        public async Task<Response> CheckUniqueness(string name, string pan, string gst, string mobile, string cin)
        {
            Response uniquenessResponse = new Response();
            int userId = -1;
            Vendor vendor = new Vendor();
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    vendor.Name = name ?? "";
                    vendor.PAN = pan ?? "";
                    vendor.CompanyIdentificationNumber = cin ?? "";
                    vendor.ContactNumber = mobile ?? "";
                    vendor.GST = gst ?? "";
                    bool isUnique = await _vendorDbContext.IsVendorPresent(vendor);
                    if (isUnique == false)
                    {
                        uniquenessResponse.Status = Status.Success;
                    }
                    else
                    {
                        uniquenessResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.DataAlreadyPresent
                        };
                    }
                    return uniquenessResponse;
                }
                else
                {
                    uniquenessResponse.Status = Status.Failure;
                    uniquenessResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                uniquenessResponse.Status = Status.Failure;
                uniquenessResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Check vendor unique", Severity.Medium, vendor, uniquenessResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (uniquenessResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendor, uniquenessResponse, "Check vendor unique", uniquenessResponse.Status, severity, userId); }).Start();
            }
            return uniquenessResponse;
        }

        [Audit("Deleted Vendor with Id","Vendor")]
        public async Task<Response> DeleteVendor(int vendorId, bool isHardDelete)
        {
            Response deleteResponse = new Response();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    if (vendorId > 0)
                    {
                        bool isDeleted = await _vendorDbContext.DeleteVendor(vendorId, isHardDelete);
                        if (isDeleted == true)
                        {
                            deleteResponse.Status = Status.Success;
                        }
                        else
                        {
                            deleteResponse.Error = new Error()
                            {
                                ErrorCode = Constants.ErrorCodes.NotFound,
                                ErrorMessage = Constants.ErrorMessages.RecordNotFound
                            };
                        }
                        return deleteResponse;
                    }
                    else
                    {
                        deleteResponse.Status = Status.Failure;
                        deleteResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.InValidId
                        };
                    }
                }
                else
                {
                    deleteResponse.Status = Status.Failure;
                    deleteResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                deleteResponse.Status = Status.Failure;
                deleteResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Deletevendor", Severity.Medium, vendorId, deleteResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (deleteResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendorId, deleteResponse, "DeletVendor", deleteResponse.Status, severity, userId); }).Start();
            }
            return deleteResponse;
        }
        public async Task<VendorsResponse> GetVendorById(int vendorId)
        {
            VendorsResponse vendorResponse = new VendorsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    vendorResponse.Vendors = new List<Vendor>();
                    vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InValidId);
                    if (vendorId > 0)
                    {
                        Vendor vendor = await _vendorDbContext.GetVendorById(vendorId);
                        if (vendor != null)
                        {
                            vendorResponse.Status = Status.Success;
                            vendorResponse.Vendors.Add(vendor);
                        }
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
                new Task(() => { _logger.Log(vendorId, vendorResponse, " GetVendorById", vendorResponse.Status, severity, userId); }).Start();
            }
            return vendorResponse;
        }
        public async Task<VendorsResponse> GetVendors(string name, int pageNumber, int pageSize)
        {
            VendorsResponse vendorResponse = new VendorsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    if (String.IsNullOrEmpty(name))
                        name = "";
                    if (pageSize <= 0 || pageNumber <= 0)
                    {
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return vendorResponse;
                    }
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    vendorResponse = await _vendorDbContext.GetVendors(name, limit, offset);
                    vendorResponse.PagingInfo.PageNumber = pageNumber;
                    vendorResponse.PagingInfo.PageSize = pageSize;
                    if (vendorResponse.Vendors == null || vendorResponse.Vendors.Count == 0)
                    {
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoVendorsYet);
                    }
                    else
                    {
                        vendorResponse.Status = Status.Success;
                    }
                }
                else
                {
                    vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UnableToFetch);
                new Task(() => { _logger.LogException(exception, "GetVendors", Severity.High, name, vendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.Medium;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(name, vendorResponse, "GetVendors", vendorResponse.Status, severity, -1); }).Start();
            }
            return vendorResponse;
        }
        [Audit("Updated Vendor","Vendor")]
        public async Task<VendorsResponse> UpdateVendor(Vendor vendor)
        {
            VendorsResponse vendorResponse = new VendorsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    vendorResponse.Vendors = new List<Vendor>();
                    if (VendorValidator.Validate(vendor))
                    {
                        vendor = await _vendorDbContext.UpdateVendor(vendor);
                        if (vendor != null)
                        {
                            vendorResponse.Status = Status.Success;
                            vendorResponse.Vendors.Add(vendor);
                        }
                        else
                        {
                            vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                        }
                        return vendorResponse;
                    }
                    else
                    {
                        vendorResponse.Status = Status.Failure;
                        vendorResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.MissingValues
                        };
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
                vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "UpdateVendor", Severity.High, vendor, vendorResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (vendorResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(vendor, vendorResponse, "UpdateVendor", vendorResponse.Status, severity, userId); }).Start();
            }
            return vendorResponse;
        }
    }
}
