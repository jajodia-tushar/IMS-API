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
                        if (deleteVendorOrderResponse.Error == null)
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
        }
        public async Task<OrdersByEmployeeIdResponse> GetEmployeeOrders(string employeeId)
        {
            OrdersByEmployeeIdResponse employeeOrdersResponse = new OrdersByEmployeeIdResponse();
            employeeOrdersResponse.Error = new Error() { };
            employeeOrdersResponse.Status = Status.Failure;
            try
            {
                GetEmployeeResponse employeeResponse = _employeeService.ValidateEmployee(employeeId);
                employeeOrdersResponse.Employee = employeeResponse.Employee;

                if (employeeOrdersResponse.Employee != null)
                {
                    List<EmployeeOrderDetails> orders = await _employeeOrderDbContext.GetOrdersByEmployeeId(employeeId);
                    if (orders.Count > 0)
                    {
                        employeeOrdersResponse.Status = Status.Success;
                        employeeOrdersResponse.EmployeeOrders = orders;
                        return employeeOrdersResponse;
                    }
                    employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoOrdersYet);

                    return employeeOrdersResponse;
                }
                employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InValidId);
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "Retrieving orders by employee id", IMS.Entities.Severity.Critical, employeeId, employeeOrdersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeOrdersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeOrdersResponse, "Retrieving orders by employee id", employeeOrdersResponse.Status, severity, -1); }).Start();
            }
            return employeeOrdersResponse;
        }


        public async Task<EmployeeOrderResponse> PlaceEmployeeOrder(EmployeeOrder employeeOrder)
        {
            EmployeeOrderResponse placeEmployeeOrderResponse = new EmployeeOrderResponse();
            placeEmployeeOrderResponse.Error = new Error() { };
            placeEmployeeOrderResponse.Status = Status.Failure;
            if (EmployeeOrderValidator.ValidateEmployeeOrder(employeeOrder) == false)
            {
                placeEmployeeOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.MissingValues);
                placeEmployeeOrderResponse.EmployeeOrder = null;
                return placeEmployeeOrderResponse;
            }
            try
            {
                GetEmployeeResponse employeeResponse = _employeeService.ValidateEmployee(employeeOrder.Employee.Id);
                if (employeeResponse.Employee != null && employeeResponse.Employee.IsActive != false)
                {
                    placeEmployeeOrderResponse.EmployeeOrder = await _employeeOrderDbContext.AddEmployeeOrder(employeeOrder);
                    if (placeEmployeeOrderResponse.EmployeeOrder != null)
                    {
                        placeEmployeeOrderResponse.Status = Status.Success;
                        return placeEmployeeOrderResponse;
                    }
                    placeEmployeeOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UnableToPlaceOrder);

                    return placeEmployeeOrderResponse;
                }
                placeEmployeeOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InValidId);

            }
            catch (Exception exception)
            {
                placeEmployeeOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UnableToPlaceOrder);
                new Task(() => { _logger.LogException(exception, "Place employee order", IMS.Entities.Severity.Critical, employeeOrder, placeEmployeeOrderResponse); }).Start();
                return placeEmployeeOrderResponse;
            }
            finally
            {
                Severity severity = Severity.No;
                if (placeEmployeeOrderResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeOrder, placeEmployeeOrderResponse, "Place employee order", placeEmployeeOrderResponse.Status, severity, -1); }).Start();
            }
            return placeEmployeeOrderResponse;
        }

        public async Task<EmployeeRecentOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize)
        {
            EmployeeRecentOrderResponse employeeRecentOrderResponse = new EmployeeRecentOrderResponse();
            int userId = -1;
            employeeRecentOrderResponse.Status = Status.Failure;
            employeeRecentOrderResponse.Error = new Error() { };
            try
            {
                pageSize = (pageSize == 0) ? 10 : (pageSize < 0) ? throw new Exception() : pageSize;
                pageNumber = (pageNumber == 0) ? 1 : (pageNumber < 0) ? throw new Exception() : pageNumber;

                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    List<EmployeeRecentOrder> employeeRecentOrders = await _employeeOrderDbContext.GetRecentEmployeeOrders(pageNumber, pageSize);
                    if (employeeRecentOrders == null || employeeRecentOrders.Count == 0)
                    {
                        employeeRecentOrderResponse.Status = Status.Failure;
                        employeeRecentOrderResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.EmptyRecentEmployeeOrderList
                        };
                    }
                    else
                    {
                        employeeRecentOrderResponse.Status = Status.Success;
                        employeeRecentOrderResponse.EmployeeRecentOrders = employeeRecentOrders;
                    }
                }
                else
                {
                    employeeRecentOrderResponse.Status = Status.Failure;
                    employeeRecentOrderResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.UnAuthorized,
                        ErrorMessage = Constants.ErrorMessages.InvalidToken
                    };
                }
            }
            catch (Exception exception)
            {
                employeeRecentOrderResponse.Error.ErrorCode = Constants.ErrorCodes.BadRequest;
                employeeRecentOrderResponse.Error.ErrorMessage = Constants.ErrorMessages.UnableToShowRecentEntries;
                return employeeRecentOrderResponse;

            }
            finally
            {
                Severity severity = Severity.Medium;
                if (employeeRecentOrderResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(new String("GET Method"), employeeRecentOrderResponse, "Employee Recent Order Entries", employeeRecentOrderResponse.Status, severity, -1); }).Start();
            }
            return employeeRecentOrderResponse;

        }
        //vendororders

        public async Task<VendorOrderResponse> SaveVendorOrder(VendorOrder vendorOrder)
        {
            VendorOrderResponse response = new VendorOrderResponse
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {

                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.TokenExpired);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                if (VendorOrderValidator.ValidatePlacedOrder(vendorOrder))
                {
                    SetTotalPriceOfItemList(vendorOrder.VendorOrderDetails.OrderItemDetails);
                    bool isOrderSaved = await _vendorOrderDbContext.Save(vendorOrder);
                    if (isOrderSaved)
                    {
                        response.Status = Status.Success;
                        response.VendorOrder = vendorOrder;
                    }
                    else
                        response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
                }
                else
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
            }
            catch (CustomException e)
            {

                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "PlaceVendorOrder", Severity.Critical, vendorOrder, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "PlaceVendorOrder", Severity.Critical, vendorOrder, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(vendorOrder, response, "placing vendor order", response.Status, severity, userId); }).Start();
            }
            return response;

        }

        public async Task<GetListOfVendorOrdersResponse> GetAllVendorOrderPendingApprovals(int pageNumber, int pageSize)
        {
            GetListOfVendorOrdersResponse response = new GetListOfVendorOrdersResponse
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.TokenExpired);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                response.ListOfVendorOrders = await _vendorOrderDbContext.GetAllPendingApprovals(pageNumber, pageSize);
                response.Status = Status.Success;
            }
            catch (CustomException e)
            {

                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "GetAllVendorPendingApprovals", Severity.Critical, null, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "GetAllVendorPendingApprovals", Severity.Critical, null, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(null, response, "Get All VendorOrder Pending Approvals", response.Status, severity, userId); }).Start();
            }
            return response;

        }


        private void SetTotalPriceOfItemList(List<ItemQuantityPriceMapping> orderItemDetails)
        {
            foreach (var itemQtyPrice in orderItemDetails)
                itemQtyPrice.TotalPrice = Math.Round(itemQtyPrice.Item.Rate * itemQtyPrice.Quantity, 2);
        }

        public async Task<Response> ApproveVendorOrder(VendorOrder vendorOrder)
        {
            Response response = new Response
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {

                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.TokenExpired);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                if (VendorOrderValidator.ValidateApproveRequest(vendorOrder))
                {

                    bool isOrderApproved = await _vendorOrderDbContext.ApproveOrder(vendorOrder);
                    if (isOrderApproved)
                    {
                        response.Status = Status.Success;

                    }
                    else
                        response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
                }
                else
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
            }
           
            catch (CustomException e)
            {

                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", Severity.Critical, vendorOrder, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", Severity.Critical, vendorOrder, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(vendorOrder, response, "Approving vendor order", response.Status, severity, userId); }).Start();
            }
            return response;

        }
    }
}
