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
    public class OrderService : IOrderService
    {

        private IVendorOrderDbContext _vendorOrderDbContext;
        private IEmployeeOrderDbContext _employeeOrderDbContext;
        private ITokenProvider _tokenProvider;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private IEmployeeService _employeeService;
        private IVendorService _vendorService;

        public OrderService(IVendorOrderDbContext vendorOrderDbContext, IEmployeeOrderDbContext employeeOrderDbContext, ITokenProvider tokenProvider, ILogManager logger, IHttpContextAccessor httpContextAccessor, IEmployeeService employeeService, IVendorService vendorService)
        {

            _vendorOrderDbContext = vendorOrderDbContext;
            _employeeOrderDbContext = employeeOrderDbContext;
            _tokenProvider = tokenProvider;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
            _vendorService = vendorService;
        }
<<<<<<< HEAD
        public async Task<Response> Delete(int orderId)
        {
            Response deleteVendorOrderResponse = new Response();
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
                        deleteVendorOrderResponse = ValidateOrderId(orderId);
                        if(deleteVendorOrderResponse.Error == null)
                        {
                            bool isDeleted = await _vendorOrderDbContext.Delete(orderId);
                            if (isDeleted)
                                deleteVendorOrderResponse.Status = Status.Success;
                            else
                            {
                                deleteVendorOrderResponse.Status = Status.Failure;
                                deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.OrderNotDeleted);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        deleteVendorOrderResponse.Status = Status.Failure;
                        deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                        new Task(() => { _logger.LogException(exception, "Delete", Severity.High, orderId, deleteVendorOrderResponse); }).Start();
                    }
                    return deleteVendorOrderResponse;
                }
                else
                {
                    deleteVendorOrderResponse.Status = Status.Failure;
                    deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                deleteVendorOrderResponse.Status = Status.Failure;
                deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Delete", Severity.Critical, orderId, deleteVendorOrderResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (deleteVendorOrderResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(orderId, deleteVendorOrderResponse, "Delete", deleteVendorOrderResponse.Status, severity, userId); }).Start();
            }
            return deleteVendorOrderResponse;
        }

        private Response ValidateOrderId(int orderId)
        {
            Response deleteVendorOrderResponse = new Response();
            if (orderId <= 0)
            {
                deleteVendorOrderResponse.Status = Status.Failure;
                deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InvalidOrderId);
            }
            return deleteVendorOrderResponse;
=======

        public Task<EmployeeRecentOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
>>>>>>> Added Controller
        }
    }
}
