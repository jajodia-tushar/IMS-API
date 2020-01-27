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
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("EmployeeOrders", Name = "GetEmployeeOrders")]
        public async Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId,int pageNumber, int pageSize, string startDate, string endDate)
        {
            EmployeeOrderResponse emloyeeOrderResponse = null;
            try
            {
                IMS.Entities.EmployeeOrderResponse employeeOrderResponseEntity = new IMS.Entities.EmployeeOrderResponse();
                if (String.IsNullOrEmpty(startDate)||String.IsNullOrEmpty(endDate))
                {
                    employeeOrderResponseEntity = await _orderService.GetEmployeeRecentOrders(pageNumber, pageSize);
                }
                else
                {
                    employeeOrderResponseEntity = await _orderService.GetEmployeeOrders(employeeId, pageNumber, pageSize, startDate, endDate);
                }
                emloyeeOrderResponse = EmployeeOrderTranslator.ToDataContractsObject(employeeOrderResponseEntity);
            }
            catch (Exception ex)
            {
                emloyeeOrderResponse = new IMS.Contracts.EmployeeOrderResponse()
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
        public async Task<VendorsOrderResponse> GetVendorOrders(int? pageNumber, int? pageSize, bool isApproved, string fromDate = null, string toDate = null)
            {
            VendorsOrderResponse dtoVendorsOrderResponse = null;
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;
                if (currentPageNumber <= 0 || currentPageSize <= 0)
                {
                    return new VendorsOrderResponse
                    {
                        Status = Status.Failure,
                        Error = new Error
                        {
                            ErrorCode = Constants.ErrorCodes.BadRequest,
                            ErrorMessage = Constants.ErrorMessages.InvalidRequest
                        }
                    };
                }
                IMS.Entities.VendorsOrderResponse entitiesResponse = await _orderService.GetVendorOrders(isApproved, currentPageNumber, currentPageSize, fromDate, toDate);
                dtoVendorsOrderResponse = VendorOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {
                dtoVendorsOrderResponse = new VendorsOrderResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "GetVendorOrders", IMS.Entities.Severity.Critical,isApproved+";"+pageNumber+";"+pageSize+";"+fromDate+";"+toDate, dtoVendorsOrderResponse); }).Start();
            }

            return dtoVendorsOrderResponse;
        }

        /// <summary>
        /// retrieves the List of vendororders by vendorId.
        /// </summary>
        /// <returns>entire List of vendorOrder object along with status</returns>
        /// <response code="200">Returns List of VendorOrder object and status success.If fails only status and error will be sent</response>
        [HttpGet("Vendors/{vendorId}", Name = "GetVendorOrdersByVendorId")]
        public async Task<VendorsOrderResponse> GetVendorOrdersByVendorId(int? pageNumber, int? pageSize, int vendorId,string fromDate = null, string toDate = null)
        {
            var dtoVendorsOrderResponse = new VendorsOrderResponse();
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;
                if (currentPageNumber <= 0 || currentPageSize <= 0)
                {
                    dtoVendorsOrderResponse.Status = Status.Failure;
                    dtoVendorsOrderResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidRequest
                    };
                }
                var doListOfVendorOrdersResponse = await _orderService.GetVendorOrdersByVendorId(vendorId, currentPageNumber, currentPageSize, fromDate, toDate);
                dtoVendorsOrderResponse = VendorOrderTranslator.ToDataContractsObject(doListOfVendorOrdersResponse);
            }
            catch (Exception e)
            {
                dtoVendorsOrderResponse.Status = Status.Failure;
                dtoVendorsOrderResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
                new Task(() => { _logger.LogException(e, "GetVendorOrdersByVendorId", IMS.Entities.Severity.Critical,vendorId+";"+pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, dtoVendorsOrderResponse); }).Start();
            }

            return dtoVendorsOrderResponse;

        }

        /// <summary>
        /// vendororder is updated and approved along this data transfer from vendororder to warehouse
        /// </summary>
        /// <param name="vendorOrder">Here vendorOrder contains two objects named vendor and vendororderdetails</param>
        /// <returns>Response</returns>
        /// <response code="200">Returns Success status  if Vendororder is approved otherwise it returns Error and status failure</response>
        // PUT: api/order/VendorOrders/PendingApprovals
        [HttpPut("VendorOrders/PendingApprovals", Name = "ApproveVendorOrder")]
        public async Task<VendorOrderResponse> ApproveVendorOrder([FromBody] VendorOrder vendorOrder)
        {
            var vendorOrderResponse = new VendorOrderResponse();
            try
            {
                IMS.Entities.VendorOrder entitiesVendorOrder = VendorOrderTranslator.ToEntitiesObject(vendorOrder);
                IMS.Entities.VendorOrderResponse entitiesVendorOrderResponse = await _orderService.ApproveVendorOrder(entitiesVendorOrder);
                vendorOrderResponse = VendorOrderTranslator.ToDataContractsObject(entitiesVendorOrderResponse);
            }
            catch (Exception e)
            {
                vendorOrderResponse = new VendorOrderResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", IMS.Entities.Severity.Critical, vendorOrder, vendorOrderResponse); }).Start();

            }
            return vendorOrderResponse;
        }
        /// <summary>
        /// Retrieves VendorOrder using orderId
        /// </summary>
        /// <returns>Response</returns>
        /// <response code="200">Returns Status Success if vendorOrder is found on the given orderId</response>
        // GET: api/order/VendorOrders/OrderId

        [Route("VendorOrders/{orderId}")]
        [HttpGet]
        public async Task<VendorOrderResponse> GetVendorOrderByOrderId(int orderId)
        {
            var dtoVendorOrderResponse = new VendorOrderResponse();
            try
            {
                var doListOfVendorOrderResponse = await _orderService.GetVendorOrderByOrderId(orderId);
                dtoVendorOrderResponse = VendorOrderTranslator.ToDataContractsObject(doListOfVendorOrderResponse);
            }
            catch (Exception e)
            {
                dtoVendorOrderResponse.Status = Status.Failure;
                dtoVendorOrderResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
                new Task(() => { _logger.LogException(e, "GetVendorOrderByOrderId", IMS.Entities.Severity.Critical, orderId, dtoVendorOrderResponse); }).Start();
            }

            return dtoVendorOrderResponse;
        }

        

        [HttpPost("EmployeeBulkOrders", Name ="CreateEmployeeBulkOrder")]
        public async Task<EmployeeBulkOrdersResponse> PlaceEmployeeBulkOrder([FromBody] EmployeeBulkOrder employeeBulkOrder)
        {
            EmployeeBulkOrdersResponse contractsEmployeeBulkOrdersResponse = null;
            try
            {
                IMS.Entities.EmployeeBulkOrder entitiesEmployeeBulkOrder = EmployeeBulkOrderTranslator.ToEntitiesObject(employeeBulkOrder);
                IMS.Entities.EmployeeBulkOrdersResponse entitiesEmployeeBulkOrdersResponse = await _orderService.PlaceEmployeeBulkOrder(entitiesEmployeeBulkOrder);
                contractsEmployeeBulkOrdersResponse = EmployeeBulkOrderTranslator.ToDataContractsObject(entitiesEmployeeBulkOrdersResponse);
            }
            catch (Exception e)
            {
                contractsEmployeeBulkOrdersResponse.Status = Status.Failure;
                contractsEmployeeBulkOrdersResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
                new Task(() => { _logger.LogException(e, "CreateEmployeeBulkOrder", IMS.Entities.Severity.Critical, employeeBulkOrder, contractsEmployeeBulkOrdersResponse); }).Start();
            }

            return contractsEmployeeBulkOrdersResponse;
        }

        [HttpGet("EmployeeBulkOrders", Name = "GetEmployeeBulkOrder")]
        public async Task<EmployeeBulkOrdersResponse> GetAllEmployeeBulkOrders(int? pageNumber = null, int? pageSize = null, string fromDate=null, string toDate =null)
        {            
            EmployeeBulkOrdersResponse contractsBulkOrdersResponse = null;
            try
            {                
                IMS.Entities.EmployeeBulkOrdersResponse entitiesResponse = await _orderService.GetEmployeeBulkOrders(pageNumber, pageSize, fromDate, toDate);
                contractsBulkOrdersResponse = EmployeeBulkOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                contractsBulkOrdersResponse = new EmployeeBulkOrdersResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "GetAllEmployeeBulkOrders", IMS.Entities.Severity.Critical,"GET /EmployeeBulkOrders", contractsBulkOrdersResponse); }).Start();
            }

            return contractsBulkOrdersResponse;
        }
        /// <summary>
        /// Get all EmployeeBulkOrders
        /// </summary>
        /// <returns>EmployeeBulkOrdersResponse</returns>
        /// <response code="200">Returns Status Success if BulkOrders were found</response>
        [HttpGet("EmployeeBulkOrders/{orderId}", Name = "GetEmployeeBulkOrderById")]
        public async Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrderById(int orderid)
        {
            EmployeeBulkOrdersResponse contractsBulkOrderResponse = null;
            try
            {
                IMS.Entities.EmployeeBulkOrdersResponse entitiesResponse = await _orderService.GetEmployeeBulkOrderById(orderid);
                contractsBulkOrderResponse = EmployeeBulkOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                contractsBulkOrderResponse = new EmployeeBulkOrdersResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "GetEmployeeBulkOrderById", IMS.Entities.Severity.Critical, "GET /EmployeeBulkOrders", contractsBulkOrderResponse); }).Start();
            }

            return contractsBulkOrderResponse;
        }

        [HttpPut("EmployeeBulkOrders/{orderId}/Approve", Name = "ApproveEmployeeBulkOrder")]
        public async Task<ApproveBulkOrderResponse> ApproveEmployeeBulkOrder(int orderid, [FromBody] ApproveEmployeeBulkOrder approveEmployeeBulkOrder)
        {
            ApproveBulkOrderResponse contractsApproveBulkOrderResponse = null;
            try
            {
                IMS.Entities.ApproveEmployeeBulkOrder entitiesApproveBulkOrder= EmployeeBulkOrderTranslator.ToEntitiesObject(approveEmployeeBulkOrder);
                IMS.Entities.ApproveBulkOrderResponse entitiesResponse = await _orderService.ApproveEmployeeBulkOrder(orderid, entitiesApproveBulkOrder);
                contractsApproveBulkOrderResponse = EmployeeBulkOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                contractsApproveBulkOrderResponse = new ApproveBulkOrderResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "ApproveEmployeeBulkOrder", IMS.Entities.Severity.Critical, approveEmployeeBulkOrder, contractsApproveBulkOrderResponse); }).Start();
            }

            return contractsApproveBulkOrderResponse;
        }
        [HttpPut("EmployeeBulkOrders/{orderId}/Reject", Name = "RejectEmployeeBulkOrder")]
        public async Task<Response> RejectEmployeeBulkOrder(int orderId)
        {
            Response response = null;
            try
            {
                
                IMS.Entities.Response entitiesResponse = await _orderService.RejectEmployeeBulkOrder(orderId);
                response = Translator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                response = new Response
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "RejectEmployeeBulkOrder", IMS.Entities.Severity.Critical, orderId, response); }).Start();
            }

            return response;
        }

        /// <summary>
        /// Return Employee Bulk Order
        /// </summary>
        /// <returns>EmployeeBulkOrdersResponse</returns>
        /// <response code="200">Returns Status Success if order return was successfull</response>
        [HttpPut("EmployeeBulkOrders/{orderId}/return", Name = "ReturnEmployeeBulkOrderById")]
        public async Task<EmployeeBulkOrdersResponse> ReturnBulkOrderById(int orderId, [FromBody] EmployeeBulkOrder employeeBulkOrder)
        {
            EmployeeBulkOrdersResponse contractsBulkOrderResponse = null;
            try
            {
                IMS.Entities.EmployeeBulkOrder entitiesEmployeeBulkOrder = EmployeeBulkOrderTranslator.ToEntitiesObject(employeeBulkOrder);
                IMS.Entities.EmployeeBulkOrdersResponse entitiesResponse = await _orderService.ReturnOrderItems(orderId, entitiesEmployeeBulkOrder);
                contractsBulkOrderResponse = EmployeeBulkOrderTranslator.ToDataContractsObject(entitiesResponse);
            }
            catch (Exception e)
            {

                contractsBulkOrderResponse = new EmployeeBulkOrdersResponse
                {
                    Status = Status.Failure,
                    Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(e, "ReturnBulkOrderById", IMS.Entities.Severity.Critical, employeeBulkOrder, contractsBulkOrderResponse); }).Start();
            }

            return contractsBulkOrderResponse;
        }
    }
}
