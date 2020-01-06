﻿using IMS.DataLayer.Interfaces;
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
                    catch (Exception exception)
                    {
                        throw exception;
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
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    if(String.IsNullOrEmpty(name))
                        name = "";
                    if (pageSize <= 0 || pageNumber <= 0)
                    {
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return vendorResponse;
                    }
                    vendorResponse.PagingInfo = new PagingInfo()
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    List<Vendor> vendors = new List<Vendor>();
                    vendorResponse.PagingInfo.TotalResults = await _vendorDbContext.GetVendors(name, limit, offset, vendors);
                    if (vendors == null || vendors.Count == 0)
                    {
                        vendorResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoVendorsYet);
                    }
                    else
                    {
                        vendorResponse.Status = Status.Success;
                        vendorResponse.Vendors = vendors;
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
    }
}
