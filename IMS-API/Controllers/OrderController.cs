using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
=======
using IMS.Contracts;
using IMS.Core;
using IMS.Core.services;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
>>>>>>> Added Controller
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
<<<<<<< HEAD
    {
        private IOrderService _orderService;
        private ILogManager _logger;
=======
    {
        private IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        /// <summary>
        /// Returns recent order placed by the employee with employee and employee order details
        /// </summary>
        /// <returns>List of employee recent order along with the status</returns>
        /// <response code="200">Returns the employee recent order along with status success </response>
        /// <response code="400">If Unable to show recent entries </response>
        /// <response code="401">If token is Invalid</response>
        /// <response code="403">If Username and Password credentials are not of Admin and SuperAdmin</response>
        // GET: api/Order/EmployeeRecentOrderDetails
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("EmployeeRecentOrderDetails", Name = "GetEmployeeRecentOrderDetails")]
        public async Task<EmployeeRecentOrderResponse> GetEmployeeRecentOrderDetails(int? pageNumber = null, int? pageSize = null)
        {

            EmployeeRecentOrderResponse dtoEmployeeRecentOrderResponse = null;
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;

                IMS.Entities.EmployeeRecentOrderResponse doEmployeeRecentOrderResponse = await _orderService.GetEmployeeRecentOrders(currentPageNumber, currentPageSize);
                dtoEmployeeRecentOrderResponse = EmployeeOrderTranslator.ToDataContractsObject(doEmployeeRecentOrderResponse);
            }
            catch (Exception ex)
            {
                dtoEmployeeRecentOrderResponse = new EmployeeRecentOrderResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return dtoEmployeeRecentOrderResponse;
        }
>>>>>>> Added Controller

        public OrderController(IOrderService orderService, ILogManager logManager)
        {
            _orderService = orderService;
            _logger = logManager;
        }

        /// <summary>
        /// Deletes the Vendor Order By OrderId
        /// </summary>
        /// <param name="orderId">'Order Id of that Particular Order</param>
        /// <response code="200">Returns status success if order is successfully deleted otherwise returns failure if order is not deleted</response>
        [Route("VendorOrder/{orderId}")]
        [HttpDelete]
        public async Task<Response> Delete(int orderId)
        {
            Response deleteVendorOrderResponse = null;
            try
            {
                IMS.Entities.Response entityDeleteVendorOrderResponse = await _orderService.Delete(orderId);
                deleteVendorOrderResponse = Translator.ToDataContractsObject(entityDeleteVendorOrderResponse);
            }
            catch(Exception exception)
            {
                deleteVendorOrderResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Delete", IMS.Entities.Severity.High, orderId, deleteVendorOrderResponse); }).Start();
            }
            return deleteVendorOrderResponse;
        }
    }
}