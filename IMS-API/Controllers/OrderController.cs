﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderService _orderService;
        private ILogManager _logger;
        public OrderController(IOrderService orderService, ILogManager logger)
        {
            _orderService = orderService;
            _logger = logger;
        }




        /// <summary>
        /// Returns orders placed by an employee, returns null if no orders and if wrong employee id, returns failure
        /// </summary>
        /// <param name="employeeId">Here employee id is used to identify the employee</param>
        /// <returns>entire list of employee orders along with the employee details and status</returns>
        /// <response code="200">Returns list of employee orders along with the employee details if employee id is valid otherwise it returns null and status failure</response>
        // GET: api/Orders/EmployeOrders/1126
        [HttpGet("EmployeeOrders/{EmployeeId}", Name = "GetEmployeeOrderById")]
        public async Task<OrdersByEmployeeIdResponse> GetOrdersByEmployeeId(string employeeId)
        {
            OrdersByEmployeeIdResponse emloyeeOrderResponse = null;
            try
            {
                IMS.Entities.OrdersByEmployeeIdResponse employeeOrderResponseEntity = await _orderService.GetEmployeeOrders(employeeId);
                emloyeeOrderResponse = EmployeeOrderTranslator.ToDataContractsObject(employeeOrderResponseEntity);
            }
            catch (Exception ex)
            {
                emloyeeOrderResponse = new IMS.Contracts.OrdersByEmployeeIdResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return emloyeeOrderResponse;
        }


        /// <summary>
        /// Returns order placed by the employee with date and order id set
        /// </summary>
        /// <returns>entire employee order along with the status</returns>
        /// <response code="200">Returns the employee order along with status success if it is placed, or failure incase it is not placed</response>
        //POST: api/Orders/EmployeOrders/
        [HttpPost("EmployeeOrders", Name = "PlaceEmployeeOrder")]
        public async Task<EmployeeOrderResponse> PlaceEmployeeOrder([FromBody] EmployeeOrder employeeOrder)
        {
            EmployeeOrderResponse employeeOrderResposne = null;
            try
            {
                IMS.Entities.EmployeeOrder employeeOrderEntity = EmployeeOrderTranslator.ToEntitiesObject(employeeOrder);
                IMS.Entities.EmployeeOrderResponse employeeOrderResponseEntity = await _orderService.PlaceEmployeeOrder(employeeOrderEntity);
                employeeOrderResposne = EmployeeOrderTranslator.ToDataContractsObject(employeeOrderResponseEntity);
            }
            catch (Exception ex)
            {
                employeeOrderResposne = new IMS.Contracts.EmployeeOrderResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return employeeOrderResposne;
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
            catch (Exception exception)
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

        //vendororders
        /// <summary>
        /// places the vendororder.returns entire object if order stores successfully otherwise failure
        /// </summary>
        /// <param name="vendorOrder">Here vendorOrder contains two objects named vendor and vendororderdetails</param>
        /// <returns>entire vendorOrder object along with status</returns>
        /// <response code="200">Returns VendorOrder object  if Vendororder is valid otherwise it returns null and status failure</response>

        // POST: api/order/VendorOrders
        [HttpPost("VendorOrders", Name = "PlaceVendorOrder")]
        public async Task<VendorOrderResponse> PlaceVendorOrder([FromBody] VendorOrder vendorOrder)
        {
            VendorOrderResponse contractsVendorOrderResponse = null;
            try
            {
                IMS.Entities.VendorOrder entitiesVendorOrderResponse = VendorOrderTranslator.ToEntitiesObject(vendorOrder);
                IMS.Entities.VendorOrderResponse entitiesResponse = await _orderService.SaveVendorOrder(entitiesVendorOrderResponse);
                contractsVendorOrderResponse = VendorOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                contractsVendorOrderResponse = new VendorOrderResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "PlaceVendorOrder", IMS.Entities.Severity.Critical, vendorOrder, contractsVendorOrderResponse); }).Start();
            }

            return contractsVendorOrderResponse;
        }
        /// <summary>
        /// retrieves the List of vendororders.
        /// </summary>
        /// <returns>entire List of vendorOrder object along with status</returns>
        /// <response code="200">Returns List of VendorOrder object and status success.If fails only status and error will be sent</response>
        // GET: api/order/VendorOrders?isApproved&pageNumbe&pageSize&fromDate&toDate
        [HttpGet("VendorOrders", Name = "GetVendorOrders")]
        public async Task<GetListOfVendorOrdersResponse> GetVendorOrders(int? pageNumber, int? pageSize, bool? isApproved, string fromDate = null, string toDate = null)
        {
            GetListOfVendorOrdersResponse contractsGetListOfVendorOrdersResponse = null;
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;
                if (currentPageNumber <= 0 || currentPageSize <= 0)
                {
                    return new GetListOfVendorOrdersResponse
                    {
                        Status = Status.Failure,
                        Error = new Error
                        {
                            ErrorCode = Constants.ErrorCodes.BadRequest,
                            ErrorMessage = Constants.ErrorMessages.InvalidRequest
                        }
                    };
                }
                IMS.Entities.GetListOfVendorOrdersResponse entitiesResponse = await _orderService.GetVendorOrders(isApproved, currentPageNumber, currentPageSize, fromDate, toDate);
                contractsGetListOfVendorOrdersResponse = VendorOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {
                contractsGetListOfVendorOrdersResponse = new GetListOfVendorOrdersResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "GetVendorOrders", IMS.Entities.Severity.Critical,isApproved+";"+pageNumber+";"+pageSize+";"+fromDate+";"+toDate, contractsGetListOfVendorOrdersResponse); }).Start();
            }

            return contractsGetListOfVendorOrdersResponse;
        }
        /// <summary>
        /// vendororder is updated and approved along this data transfer from vendororder to warehouse
        /// </summary>
        /// <param name="vendorOrder">Here vendorOrder contains two objects named vendor and vendororderdetails</param>
        /// <returns>Response</returns>
        /// <response code="200">Returns Success status  if Vendororder is approved otherwise it returns Error and status failure</response>

        // PUT: api/order/VendorOrders/PendingApprovals
        [HttpPut("VendorOrders/PendingApprovals", Name = "ApproveVendorOrder")]
        public async Task<Response> ApproveVendorOrder([FromBody] VendorOrder vendorOrder)
        {
            Response contractsResponse = null;
            try
            {
                IMS.Entities.VendorOrder entitiesVendorOrder = VendorOrderTranslator.ToEntitiesObject(vendorOrder);
                IMS.Entities.Response entitiesResponse = await _orderService.ApproveVendorOrder(entitiesVendorOrder);
                contractsResponse = Translator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {
                contractsResponse = new Response
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", IMS.Entities.Severity.Critical, vendorOrder, contractsResponse); }).Start();

            }

            return contractsResponse;

        }
    }
}
