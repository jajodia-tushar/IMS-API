using IMS.Entities;
using IMS.Core.services;
using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IMS.Core.Validators;
using IMS.Core;

namespace IMS.UnitTesting.CoreTests
{
    public class VendorOrderTests
    {
        private Mock<IVendorService> _moqVendorServices;
        private Mock<IVendorOrderDbContext> _moqVendorOrderDbContext;
        private Mock<IHttpContextAccessor> _moqHttpContextAccessor;
        private Mock<ITokenProvider> _moqTokenProvider;
        private Mock<IEmployeeBulkOrderDbContext> _moqEmployeeBulkOrderDbContext;
        private Mock<IEmployeeOrderDbContext> _moqEmployeeOrderDbContext;
        private Mock<ILogManager> _moqLogger;
        private Mock<IEmployeeService> _moqEmployeeService;
        private Mock<IMailService> _moqMailService;
        private IOrderService _orderService;
        private DefaultHttpContext _context;
        private string _fromDate;
        private string _toDate;
        private DateTime _startDate;
        private DateTime _endDate;
        public VendorOrderTests()
        {
            _moqVendorServices = new Mock<IVendorService>();
            _moqVendorOrderDbContext = new Mock<IVendorOrderDbContext>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqEmployeeService = new Mock<IEmployeeService>();
            _moqEmployeeOrderDbContext = new Mock<IEmployeeOrderDbContext>();
            _moqEmployeeBulkOrderDbContext = new Mock<IEmployeeBulkOrderDbContext>();
            _moqMailService = new Mock<IMailService>();
            _moqLogger = new Mock<ILogManager>();
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            _context = new DefaultHttpContext();
            _fromDate = "20180101";
            _toDate = "20210101";
            ReportsValidator.InitializeAndValidateDates(_fromDate, _toDate, out _startDate, out _endDate);
            _context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(_context);
            _orderService = new OrderService(_moqVendorOrderDbContext.Object, _moqEmployeeOrderDbContext.Object, _moqTokenProvider.Object, _moqLogger.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorServices.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
        }
        [Fact]
        public async void Return_Records_Not_Found_Error_For_Empty_List_Of_Orders()
        {
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrders(false, 1, 10, _startDate, _endDate)).Returns(GetVendorOrders(0));
            var vendorsOrdersResponse = await _orderService.GetVendorOrders(false, 1, 10, _fromDate, _toDate);
            Assert.Equal(Status.Failure, vendorsOrdersResponse.Status);
            Assert.Equal(Constants.ErrorMessages.RecordNotFound, vendorsOrdersResponse.Error.ErrorMessage);
            Assert.Equal(Constants.ErrorCodes.ResourceNotFound, vendorsOrdersResponse.Error.ErrorCode);
        }

        [Fact]
        public async void Return_Status_Success_For_Getting_Vendor_Orders()
        {
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrders(false, 1, 10, _startDate, _endDate)).Returns(GetVendorOrders(2));
            var vendorsOrdersResponse = await _orderService.GetVendorOrders(false, 1, 10, _fromDate, _toDate);
            Assert.Equal(Status.Success, vendorsOrdersResponse.Status);
        }

        [Fact]
        public async void Return_Failure_For_Vendor_Order_By_OrderId_Not_Found()
        {
            int orderId = 2;
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrdersByOrderId(orderId)).Returns(GetVendorOrderByOrderId(orderId));
            _moqVendorOrderDbContext.Setup(m => m.CheckUserEditedOrderBefore(29, orderId)).Returns(Task.FromResult(false));
            _moqVendorOrderDbContext.Setup(m => m.GetLastOrderModifiedUser(orderId)).Returns(Task.FromResult("Name"));
            var vendorOrder = await _orderService.GetVendorOrderByOrderId(orderId);
            Assert.Equal(Status.Failure, vendorOrder.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, vendorOrder.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.OrderNotFound, vendorOrder.Error.ErrorMessage);
        }

        [Fact]
        public async void Returns_Success_If_OrderId_Is_Found()
        {
            int orderId = 1;
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrdersByOrderId(orderId)).Returns(GetVendorOrderByOrderId(orderId));
            _moqVendorOrderDbContext.Setup(m => m.CheckUserEditedOrderBefore(29, orderId)).Returns(Task.FromResult(false));
            _moqVendorOrderDbContext.Setup(m => m.GetLastOrderModifiedUser(orderId)).Returns(Task.FromResult("Name"));
            var vendorOrder = await _orderService.GetVendorOrderByOrderId(orderId);
            Assert.Equal(Status.Success, vendorOrder.Status);
        }

        [Fact]
        public async void Recieved_By_Returned_For_LastModifiedBy_If_Order_Not_Modified()
        {
            int orderId = 1;
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrdersByOrderId(orderId)).Returns(GetVendorOrderByOrderId(orderId));
            _moqVendorOrderDbContext.Setup(m => m.CheckUserEditedOrderBefore(29, orderId)).Returns(Task.FromResult(false));
            _moqVendorOrderDbContext.Setup(m => m.GetLastOrderModifiedUser(orderId)).Returns(Task.FromResult(""));
            var vendorOrderResponse = await _orderService.GetVendorOrderByOrderId(orderId);
            Assert.Equal("Rakesh Kumar",vendorOrderResponse.LastModifiedBy);
        }

        [Fact]
        public async void Last_Modified_User_Returned_For_If_Order_Is_Modified()
        {
            int orderId = 1;
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrdersByOrderId(orderId)).Returns(GetVendorOrderByOrderId(orderId));
            _moqVendorOrderDbContext.Setup(m => m.CheckUserEditedOrderBefore(29, orderId)).Returns(Task.FromResult(false));
            _moqVendorOrderDbContext.Setup(m => m.GetLastOrderModifiedUser(orderId)).Returns(Task.FromResult("Ekta Singh"));
            var vendorOrderResponse = await _orderService.GetVendorOrderByOrderId(orderId);
            Assert.Equal("Ekta Singh", vendorOrderResponse.LastModifiedBy);
        }
        private async Task<VendorOrder> GetVendorOrderByOrderId(int orderId)
        {
            VendorOrder vendorOrder = null;
            if (orderId == 1)
            {
                vendorOrder = new VendorOrder()
                {
                    Vendor = GetVendor(),
                    VendorOrderDetails = GetVendorOrderDetails()
                };
            }
            return vendorOrder;
        }

        private async Task<VendorOrdersDto> GetVendorOrders(int numberOfVendorOrders)
        {
            return new VendorOrdersDto()
            {
                VendorOrders = GetVendorOrdersList(numberOfVendorOrders),
                TotalRecords = numberOfVendorOrders
            };
        }

        private List<Entities.VendorOrder> GetVendorOrdersList(int size)
        {
            var vendorOrders = new List<Entities.VendorOrder>();
            for(int i = 0; i < size; i++)
            {
                var vendorOrder = new VendorOrder()
                {
                    Vendor = GetVendor(),
                    VendorOrderDetails = GetVendorOrderDetails()
                };
                vendorOrders.Add(vendorOrder);
            }
            return vendorOrders;
        }

        private VendorOrderDetails GetVendorOrderDetails()
        {
            return new VendorOrderDetails()
            {
                ChallanImageUrl = "sdf",
                ChallanNumber = "asdf",
                Date = new DateTime(),
                InvoiceImageUrl = "asdf",
                InvoiceNumber = "asddf",
                RecievedBy = "Rakesh Kumar",
                SubmittedTo = "asdf",
                IsApproved = false,
                OrderId = 1,
                TaxableAmount = 122,
                OrderItemDetails = GetOrderItemDetails()
            };
        }

        private List<ItemQuantityPriceMapping> GetOrderItemDetails()
        {
            var itemQuantitiyPriceMappings = new List<ItemQuantityPriceMapping>();
            itemQuantitiyPriceMappings.Add(GetItemQuantityPriceMapping());
            return itemQuantitiyPriceMappings;
        }

        private ItemQuantityPriceMapping GetItemQuantityPriceMapping()
        {
            return new ItemQuantityPriceMapping()
            {
                Item = GetItem(),
                Quantity = 3,
                TotalPrice = 65,
            };
        }

        private Item GetItem()
        {
            return new Item()
            {
                Id = 1,
                ImageUrl = "asdf",
                IsActive = true,
                MaxLimit = 4,
                Name = "Pen",
                Rate = 23,
                ShelvesAmberLimit = 23,
                ShelvesRedLimit = 12,
                WarehouseAmberLimit = 11,
                WarehouseRedLimit = 9,
            };
        }

        private Vendor GetVendor()
        {
            return new Vendor()
            {
                Title = "asdf",
                GST = "asdf",
                Address = "asdf",
                CompanyIdentificationNumber = "fda",
                ContactNumber = "fsda",
                Id = 1,
                Name = "Rakesh Kumar",
                PAN = "dss"
            };
        }
    }
}
