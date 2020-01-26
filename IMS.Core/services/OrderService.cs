using IMS.Core.Validators;
using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class OrderService : IOrderService
    {
        private IEmployeeBulkOrderDbContext _employeeBulkOrderDbContext;
        private IVendorOrderDbContext _vendorOrderDbContext;
        private IEmployeeOrderDbContext _employeeOrderDbContext;
        private ITokenProvider _tokenProvider;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private IEmployeeService _employeeService;
        private IVendorService _vendorService;
        private IMailService _mailService;

        public OrderService(IVendorOrderDbContext vendorOrderDbContext, IEmployeeOrderDbContext employeeOrderDbContext, ITokenProvider tokenProvider, ILogManager logger, IHttpContextAccessor httpContextAccessor, IEmployeeService employeeService, IVendorService vendorService, IMailService mailService,IEmployeeBulkOrderDbContext employeeBulkOrderDbContext)
        {
            _employeeBulkOrderDbContext = employeeBulkOrderDbContext;
            _vendorOrderDbContext = vendorOrderDbContext;
            _employeeOrderDbContext = employeeOrderDbContext;
            _tokenProvider = tokenProvider;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
            _vendorService = vendorService;
            _mailService = mailService;
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
                    bool hasUserEditedBefore = await _vendorOrderDbContext.CheckUserEditedOrderBefore(userId, orderId);
                    if (hasUserEditedBefore)
                    {
                        deleteVendorOrderResponse.Status = Status.Failure;
                        deleteVendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.UnAuthorized);

                    }
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
                                return deleteVendorOrderResponse;
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
        public async Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId, int pageNumber, int pageSize, string startDate, string endDate)
        {
            EmployeeOrderResponse employeeOrdersResponse = new EmployeeOrderResponse();
            employeeOrdersResponse.Error = new Error() { };
            employeeOrdersResponse.Status = Status.Failure;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    if (pageNumber <= 0 || pageSize <= 0)
                    {
                        employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return employeeOrdersResponse;
                    }
                    if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate) && ReportsValidator.ValidateDate(startDate, endDate))
                    {
                        GetEmployeeResponse employeeResponse = new GetEmployeeResponse();
                        if (!String.IsNullOrEmpty(employeeId))
                        {
                            employeeResponse = await _employeeService.ValidateEmployee(employeeId);
                            if (employeeResponse.Employee == null)
                            {
                                employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InValidId);
                                return employeeOrdersResponse;
                            }
                        }
                        else
                        {
                            employeeId = "";
                        }
                        int limit = pageSize;
                        int offset = (pageNumber - 1) * pageSize;
                        employeeOrdersResponse = await _employeeOrderDbContext.GetEmployeeOrders(employeeId, limit, offset, startDate, endDate);
                        if (employeeOrdersResponse.EmployeeOrders.Count != 0)
                        {
                            employeeOrdersResponse.Status = Status.Success;
                            employeeOrdersResponse.PagingInfo.PageNumber = pageNumber;
                            employeeOrdersResponse.PagingInfo.PageSize = pageSize;
                            return employeeOrdersResponse;
                        }
                        employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoOrdersYet);
                        return employeeOrdersResponse;
                    }
                    else
                    {
                        employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InvalidDate);
                    }
                }
                else
                {
                    employeeOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "Retrieving employee orders", IMS.Entities.Severity.Critical, employeeId, employeeOrdersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeOrdersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeOrdersResponse, "Retrieving employee orders", employeeOrdersResponse.Status, severity, -1); }).Start();
            }
            return employeeOrdersResponse;
        }
        public async Task<EmployeeOrderResponse> PlaceEmployeeOrder(EmployeeOrder employeeOrder)
        {
            EmployeeOrderResponse placeEmployeeOrderResponse = new EmployeeOrderResponse();
            placeEmployeeOrderResponse.Error = new Error() { };
            placeEmployeeOrderResponse.Status = Status.Failure;
            placeEmployeeOrderResponse.EmployeeOrders = new List<EmployeeOrder>();
            if (EmployeeOrderValidator.ValidateEmployeeOrder(employeeOrder) == false)
            {
                placeEmployeeOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.MissingValues);
                placeEmployeeOrderResponse.EmployeeOrders = null;
                return placeEmployeeOrderResponse;
            }
            try
            {
                GetEmployeeResponse employeeResponse = await _employeeService.ValidateEmployee(employeeOrder.Employee.Id);
                if (employeeResponse.Employee != null && employeeResponse.Employee.IsActive != false)
                {
                    EmployeeOrder placedOrder= await _employeeOrderDbContext.AddEmployeeOrder(employeeOrder);       
                    if (placedOrder!= null)
                    {
                        placeEmployeeOrderResponse.EmployeeOrders.Add(placedOrder);
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
                if (placeEmployeeOrderResponse.Status == Status.Success)
                {
                    new Task(() => {_mailService.SendEmployeeOrderReciept(placeEmployeeOrderResponse.EmployeeOrders[0]); }).Start();
                }
            }
            return placeEmployeeOrderResponse;
        }

        public async Task<EmployeeOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize)
        {
            EmployeeOrderResponse employeeRecentOrderResponse = new EmployeeOrderResponse
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    if (pageNumber <= 0)
                    {
                        pageNumber = 1;
                    }
                    if (pageSize <= 0)
                    {
                        pageSize = 10;
                    }
                    employeeRecentOrderResponse = await _employeeOrderDbContext.GetRecentEmployeeOrders(pageSize,pageNumber);
                    if (employeeRecentOrderResponse != null)
                    {
                        employeeRecentOrderResponse.Status = Status.Success;
                    }
                    else
                    {
                        employeeRecentOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.EmptyRecentEmployeeOrderList);
                    }
                }
                else
                {
                    employeeRecentOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                employeeRecentOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetEmployeeRecentOrders", Severity.Critical, pageNumber + ";" + pageSize, employeeRecentOrderResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.Medium;
                if (employeeRecentOrderResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(pageNumber + ";" + pageSize, employeeRecentOrderResponse, "GetEmployeeRecentOrders", employeeRecentOrderResponse.Status, severity, userId); }).Start();
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
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
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

        public async Task<VendorsOrderResponse> GetVendorOrders(bool isApproved, int pageNumber, int pageSize, string fromDate, string toDate)
        {
            var response = new VendorsOrderResponse();
            response.Status = Status.Failure;
            if (ReportsValidator.InitializeAndValidateDates(fromDate, toDate, out var startDate, out var endDate) == false)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidDate);
                return response;
            }
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                VendorOrdersDto vendorOrdersDto = await _vendorOrderDbContext.GetVendorOrders(isApproved, pageNumber, pageSize, startDate, endDate);
                response.VendorOrders = vendorOrdersDto.VendorOrders;
                response.PagingInfo = new PagingInfo()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalResults = vendorOrdersDto.TotalRecords
                };
                if (response.VendorOrders.Count < 1)
                {
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ResourceNotFound, Constants.ErrorMessages.RecordNotFound);
                    return response;
                }
                response.Status = Status.Success;
            }
            catch (CustomException e)
            {
                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "GetVendorOrders", Severity.Critical, isApproved + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "GetVendorOrders", Severity.Critical, isApproved + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(isApproved + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response, "GetVendorOrders", response.Status, severity, userId); }).Start();
            }
            return response;
        }

        private void SetTotalPriceOfItemList(List<ItemQuantityPriceMapping> orderItemDetails)
        {
            foreach (var itemQtyPrice in orderItemDetails)
                itemQtyPrice.TotalPrice = Math.Round(itemQtyPrice.Item.Rate * itemQtyPrice.Quantity, 2);
        }

        public async Task<VendorOrderResponse> ApproveVendorOrder(VendorOrder vendorOrder)
        {
            var vendorOrderResponse = new VendorOrderResponse();
            vendorOrderResponse.Status = Status.Failure;
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                var response =  await RestrictOrderApproval(vendorOrder, user);
                if (response.Status.Equals(Status.Success))
                {
                    vendorOrderResponse = await GetVendorOrderByOrderId(vendorOrder.VendorOrderDetails.OrderId);
                    return vendorOrderResponse;
                }
                vendorOrderResponse.Status = response.Status;
                vendorOrderResponse.Error = response.Error;
            }
            catch (CustomException e)
            {

                vendorOrderResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", Severity.Critical, vendorOrder, vendorOrderResponse); }).Start();
            }
            catch (Exception e)
            {
                vendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "ApproveVendorOrder", Severity.Critical, vendorOrder, vendorOrderResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (vendorOrderResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(vendorOrder, vendorOrderResponse, "ApproveVendorOrder", vendorOrderResponse.Status, severity, userId); }).Start();
            }
            return vendorOrderResponse;
        }

        private async Task<Response> RestrictOrderApproval(VendorOrder vendorOrder, User user)
        {
            try
            {
                var response = new Response();
                response.Status = Status.Failure;
                if (user.Role.Id.Equals(4))
                    return await ApproveOrder(vendorOrder);
                else if (user.Role.Id.Equals(1))
                {
                    bool hasUserEditedBefore = await _vendorOrderDbContext.CheckUserEditedOrderBefore(user.Id, vendorOrder.VendorOrderDetails.OrderId);
                    bool isOrderEdited = await CheckOrderEdited(vendorOrder);
                    if (!isOrderEdited)
                    {
                        if (!hasUserEditedBefore)
                            return await ApproveOrder(vendorOrder);
                        response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.AlreadyEdited);
                        return response;
                    }
                    if (!hasUserEditedBefore)
                        return await EditOrder(vendorOrder, user);
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.AlreadyEdited);
                    return response;
                }
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.UnAuthorized);
                return response;
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        private async Task<Response> EditOrder(VendorOrder vendorOrder, User user)
        {
            var response = new Response();
            response.Status = Status.Failure;
            bool isOrderEdited = await _vendorOrderDbContext.EditOrder(vendorOrder, user);
            if (isOrderEdited)
            {
                response.Status = Status.Success;
                return response;
            }
            response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
            return response;
        }

        private async Task<Response> ApproveOrder(VendorOrder vendorOrder)
        {
            var response = new Response();
            response.Status = Status.Failure;
            bool isOrderApproved = await _vendorOrderDbContext.ApproveOrder(vendorOrder);
            if (isOrderApproved)
            {
                response.Status = Status.Success;
                return response;
            }
            response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrder);
            return response;
        }

        //returns true if order is edited
        private async Task<bool> CheckOrderEdited(VendorOrder vendorOrder)
        {
            try
            {
                VendorOrderResponse vendorOrderResponse = await GetVendorOrderByOrderId(vendorOrder.VendorOrderDetails.OrderId);
                string originalDataHash = Utility.GenerateKey(vendorOrderResponse.VendorOrder);
                string editedDataHash = Utility.GenerateKey(vendorOrder);
                return originalDataHash != editedDataHash;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<VendorsOrderResponse> GetVendorOrdersByVendorId(int vendorId, int pageNumber, int pageSize, string fromDate, string toDate)
        {
            var response = new VendorsOrderResponse();
            response.Status = Status.Failure;
            response.PagingInfo = new PagingInfo()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalResults = 0
            };
            if (ReportsValidator.InitializeAndValidateDates(fromDate, toDate, out var startDate, out var endDate) == false)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidDate);
                return response;
            }
            int userId = -1;
            try
            {
                bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                if (!isTokenPresentInHeader)
                    throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (!isValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User user = Utility.GetUserFromToken(token);
                userId = user.Id;
                var vendorResponse = await _vendorService.GetVendorById(vendorId);
                if (vendorResponse.Status.Equals(Status.Failure))
                {
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InValidId);
                    return response;
                }
                var vendorsOrderResponse = await _vendorOrderDbContext.GetVendorOrdersByVendorId(vendorId, pageNumber, pageSize, startDate, endDate);
                if (vendorsOrderResponse.VendorOrders!=null && vendorsOrderResponse.VendorOrders.Count > 0)
                {
                    response.Status = Status.Success;
                    response.VendorOrders = vendorsOrderResponse.VendorOrders;
                    response.PagingInfo.TotalResults = vendorsOrderResponse.PagingInfo.TotalResults;
                    return response;
                }
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ResourceNotFound, Constants.ErrorMessages.RecordNotFound);
            }
            catch (CustomException e)
            {
                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "GetVendorOrdersByVendorId", Severity.Critical, vendorId + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "GetVendorOrdersByVendorId", Severity.Critical, vendorId + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(vendorId + ";" + pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, response, "GetVendorOrdersByVendorId", response.Status, severity, userId); }).Start();
            }
            return response;
        }

        public async Task<VendorOrderResponse> GetVendorOrderByOrderId(int orderId)
        {
            {
                var vendorOrderResponse = new VendorOrderResponse();
                vendorOrderResponse.Status = Status.Failure;
                vendorOrderResponse.CanEdit = false;
                int userId = -1;
                try
                {
                    bool isTokenPresentInHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Length > 1;
                    if (!isTokenPresentInHeader)
                        throw new InvalidTokenException(Constants.ErrorMessages.NoToken);
                    string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                    bool isValidToken = await _tokenProvider.IsValidToken(token);
                    if (!isValidToken)
                        throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    var vendorOrder = await _vendorOrderDbContext.GetVendorOrdersByOrderId(orderId);
                    if (vendorOrder.VendorOrderDetails != null)
                    {
                        vendorOrderResponse.VendorOrder = vendorOrder;
                        vendorOrderResponse.CanEdit = !await _vendorOrderDbContext.CheckUserEditedOrderBefore(user.Id, orderId);
                        return vendorOrderResponse;
                    }
                    vendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.OrderNotFount);
                    return vendorOrderResponse;
                }
                catch (CustomException e)
                {
                    vendorOrderResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                    new Task(() => { _logger.LogException(e, "GetVendorOrderByOrderId", Severity.Critical, orderId, vendorOrderResponse); }).Start();
                }
                catch (Exception e)
                {
                    vendorOrderResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                    new Task(() => { _logger.LogException(e, "GetVendorOrderByOrderId", Severity.Critical, orderId, vendorOrderResponse); }).Start();
                }
                finally
                {
                    Severity severity = Severity.No;
                    if (vendorOrderResponse.Status == Status.Failure)
                        severity = Severity.Critical;
                    new Task(() => { _logger.Log(orderId, vendorOrderResponse, "GetVendorOrderByOrderId", vendorOrderResponse.Status, severity, userId); }).Start();
                }
                return vendorOrderResponse;
            }
        }

        public async Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrders(int? pageNumber, int? pageSize, string fromDate, string toDate)
         {
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 10;
            var pageInfo = new PagingInfo()
            {
                PageNumber = currentPageNumber,
                PageSize = currentPageSize,
                TotalResults = 0
            };
            EmployeeBulkOrdersResponse employeeBulkOrdersResponse = new EmployeeBulkOrdersResponse() {
                Status = Status.Failure,
                PagingInfo = pageInfo
            };
            int userId = -1;
            
            try
            {

                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException();
                userId = request.User.Id;

                if (ReportsValidator.InitializeAndValidateDates(fromDate, toDate, out var startDate, out var endDate) == false)
                    throw new InvalidDateFormatException(Constants.ErrorMessages.InvalidDate);
                if (currentPageNumber <= 0 || currentPageSize <= 0)
                    throw new InvalidPagingInfo(Constants.ErrorMessages.InvalidPagingDetails);

                Tuple<int,List<EmployeeBulkOrder>> bulkOrdersResultFromDb = await _employeeBulkOrderDbContext.GetAllEmployeeBulkOrders(currentPageNumber, currentPageSize, startDate, endDate);
                if (bulkOrdersResultFromDb.Item2.Count < 0)
                    throw new RecordsNotFoundException(Constants.ErrorMessages.RecordNotFound);
                
                pageInfo.TotalResults = bulkOrdersResultFromDb.Item1;
                employeeBulkOrdersResponse.Status = Status.Success;
                employeeBulkOrdersResponse.EmployeeBulkOrders = bulkOrdersResultFromDb.Item2;
                
            }
            catch(CustomException exception)
            {
                employeeBulkOrdersResponse.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "GetEmployeeBulkOrders", IMS.Entities.Severity.Critical, "GET EmployeeBulkOrders/", employeeBulkOrdersResponse); }).Start();
            }
           
            catch (Exception exception)
            {
                employeeBulkOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetEmployeeBulkOrders", IMS.Entities.Severity.Critical,"GET EmployeeBulkOrders/", employeeBulkOrdersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeBulkOrdersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("GET EmployeeBulkOrders/", employeeBulkOrdersResponse, "GetEmployeeBulkOrders", employeeBulkOrdersResponse.Status, severity, userId); }).Start();
            }
            return employeeBulkOrdersResponse;
        }


        public async Task<EmployeeBulkOrdersResponse> PlaceEmployeeBulkOrder(EmployeeBulkOrder employeeBulkOrder)
        {
            EmployeeBulkOrdersResponse response = new EmployeeBulkOrdersResponse
            {
                Status = Status.Failure
            };          
            try
            {
                if (!EmployeeBulkOrderValidator.Validate(employeeBulkOrder))
                    throw new InvalidOrderException(Constants.ErrorMessages.InvalidOrder);
                bool isOrderSaved = await _employeeBulkOrderDbContext.SaveOrder(employeeBulkOrder);
                if (!isOrderSaved)
                    throw new InvalidOrderException(Constants.ErrorMessages.InvalidOrder);
                response.Status = Status.Success;
                response.EmployeeBulkOrders = new List<EmployeeBulkOrder>();
                response.EmployeeBulkOrders.Add(employeeBulkOrder);
            }
            catch (CustomException e)
            {

                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "PlaceEmployeeBulkOrder", Severity.Critical, employeeBulkOrder, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "PlaceEmployeeBulkOrder", Severity.Critical, employeeBulkOrder, response); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeBulkOrder, response, "PlaceEmployeeBulkOrder", response.Status, severity, -1); }).Start();
                if (response.Status == Status.Success)
                {
                    new Task(() => { _mailService.SendEmployeeBulkOrderReciept(employeeBulkOrder,BulkOrderRequestStatus.Pending); }).Start();
                }
            }
            return response;

        }

        public async Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrderById(int orderId)
        {
            
            EmployeeBulkOrdersResponse employeeBulkOrdersResponse = new EmployeeBulkOrdersResponse()
            {
                Status = Status.Failure,
            };
            int userId = -1;

            try
            {

                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException();
                userId = request.User.Id;
                if (orderId<=0)
                    throw new InvalidOrderException(Constants.ErrorMessages.InvalidOrderId);
                EmployeeBulkOrder order = await _employeeBulkOrderDbContext.GetOrderById(orderId);
                if (order != null)
                {
                    employeeBulkOrdersResponse.EmployeeBulkOrders = new List<EmployeeBulkOrder>();
                    employeeBulkOrdersResponse.EmployeeBulkOrders.Add(order);
                    employeeBulkOrdersResponse.Status = Status.Success;
                }
                else
                    employeeBulkOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.OrderNotFound);
                                    

            }
            catch (CustomException exception)
            {
                employeeBulkOrdersResponse.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "GetEmployeeBulkOrderById", IMS.Entities.Severity.Critical, orderId, employeeBulkOrdersResponse); }).Start();
            }

            catch (Exception exception)
            {
                employeeBulkOrdersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetEmployeeBulkOrderById", IMS.Entities.Severity.Critical, orderId, employeeBulkOrdersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeBulkOrdersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(orderId, employeeBulkOrdersResponse, "GetEmployeeBulkOrders", employeeBulkOrdersResponse.Status, severity, userId); }).Start();
            }
            return employeeBulkOrdersResponse;
        }
    }
}
