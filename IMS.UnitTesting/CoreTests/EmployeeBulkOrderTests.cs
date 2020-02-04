using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class EmployeeBulkOrderTests
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
        public EmployeeBulkOrderTests()
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
            _orderService = new OrderService(_moqVendorOrderDbContext.Object, _moqEmployeeOrderDbContext.Object, _moqTokenProvider.Object, _moqLogger.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorServices.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);

        }
        public void ValidateToken(bool isValidToken, string token)
        {
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(isValidToken));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + token;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async void Return_Failure_For_Bulk_Order_By_OrderId_Is_Invalid()
        {
            int orderId = -1;
            ValidateToken(true, Tokens.SuperAdmin);
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Failure_For_Bulk_Order_By_OrderId_NotFound()
        {
            int orderId = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(1)).Returns(Task.FromResult((EmployeeBulkOrder)null));
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Success_For_Bulk_Order_By_OrderId_Is_Found()
        {
            int orderId = 2;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(2)).Returns(Task.FromResult(new EmployeeBulkOrder() { BulkOrderId= orderId }));
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            Assert.Equal(Status.Success, response.Status);
            Assert.Equal(orderId,response.EmployeeBulkOrders[0].BulkOrderId);
        }
        [Fact]
        public async void Return_Failure_For_Get_All_Bulk_Order_If_Pagenumber_Or_Pagesize_Are_Invalid()
        {
            string fromDate = "20180101";
            string toDate = "20210101";
            ValidateToken(true, Tokens.SuperAdmin);
            var response = await _orderService.GetEmployeeBulkOrders(-1,-1,fromDate,toDate);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Failure_For_Get_All_Bulk_Order_If_Orders_Not_Present()
        {
            string fromDate = "20180101";
            string toDate = "20210101";
            int pagenumber = 1;
            int pagesize = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetAllEmployeeBulkOrders(pagenumber,pagesize,It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(Tuple.Create<int, List<EmployeeBulkOrder>>(0, new List<EmployeeBulkOrder>())));
            var response = await _orderService.GetEmployeeBulkOrders(1, 1, fromDate, toDate);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }

        [Fact]
        public async void Return_Success_For_Get_All_Bulk_Order_If_Orders_Present()
        {
            string fromDate = "20180101";
            string toDate = "20210101";
            int pagenumber = 1;
            int pagesize = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetAllEmployeeBulkOrders(pagenumber, pagesize, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(Tuple.Create<int, List<EmployeeBulkOrder>>(1, new List<EmployeeBulkOrder>() { new EmployeeBulkOrder { BulkOrderId = 1 } })));
            var response = await _orderService.GetEmployeeBulkOrders(1, 1, fromDate, toDate);
            Assert.Equal(Status.Success, response.Status);
         }
    }
}
