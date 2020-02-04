using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class EmployeeOrderTesting
    {
        public Mock<IEmployeeOrderDbContext> _moqOrderDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;
        public Mock<IEmployeeBulkOrderDbContext> _moqEmployeeBulkOrderDbContext;
        public Mock<IVendorOrderDbContext> _moqVendorOrderDbContext;
        public Mock<IEmployeeService> _moqEmployeeService;
        public Mock<IVendorService> _moqVendorService;
        public Mock<IMailService> _moqMailService;

        public EmployeeOrderTesting()
        {
            _moqOrderDbContext = new Mock<IEmployeeOrderDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _moqEmployeeBulkOrderDbContext = new Mock<IEmployeeBulkOrderDbContext>();
            _moqEmployeeService = new Mock<IEmployeeService>();
            _moqVendorOrderDbContext = new Mock<IVendorOrderDbContext>();
            _moqVendorService = new Mock<IVendorService>();
            _moqMailService = new Mock<IMailService>();
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }
        [Fact]
        public async void GetEmployeeOrders_Should_Return_Success_When_Request_Is_Valid()
        {
            _moqEmployeeService.Setup(m=>m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.GetEmployeeOrders(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new EmployeeOrderResponse() { Status = Status.Success, EmployeeOrders=new List<EmployeeOrder>() }));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object,_moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object,_moqEmployeeService.Object,_moqVendorService.Object,_moqMailService.Object,_moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeOrders("1126", 1, 1, "20200101", "20200203");
            Assert.Equal(Status.Success, resultant.Status);
        }
        [Fact]
        public async void GetEmployeeOrders_Should_Return_Error_When_RequestDate_Is_InValid()
        {
            _moqEmployeeService.Setup(m => m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.GetEmployeeOrders(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new EmployeeOrderResponse() { Status = Status.Success, EmployeeOrders = new List<EmployeeOrder>() }));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeOrders("1126", 1, 1, "202001", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void GetEmployeeOrders_Should_Return_Error_When_RequestPaging_Is_InValid()
        {
            _moqEmployeeService.Setup(m => m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.GetEmployeeOrders(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new EmployeeOrderResponse() { Status = Status.Success, EmployeeOrders = new List<EmployeeOrder>() }));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeOrders("1126", 0, 0, "202001", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void GetEmployeeOrders_Should_Return_Error_When_Token_Is_InValid()
        {
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(false));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeOrders("1126", 0, 0, "202001", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void PlaceEmployeeOrder_Should_Return_Error_When_Request_Is_InValid()
        {
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.PlaceEmployeeOrder(new EmployeeOrder());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void PlaceEmployeeOrder_Should_Return_Sucess_When_Request_Is_Valid()
        {
            _moqEmployeeService.Setup(m => m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.AddEmployeeOrder(GetValidEmployeeOrder())).Returns(Task.FromResult(GetValidEmployeeOrder()));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.PlaceEmployeeOrder(new EmployeeOrder());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void GetRecentEmployeeOrder_Should_Return_Sucess_When_Request_Is_Valid()
        {
            _moqEmployeeService.Setup(m => m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.GetRecentEmployeeOrders(It.IsAny<int>(),It.IsAny<int>())).Returns(Task.FromResult(GetValidEmployeeOrderResponse()));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeRecentOrders(1, 1);
            Assert.Equal(Status.Success, resultant.Status);
        }
        [Fact]
        public async void GetRecentEmployeeOrder_Should_Return_Failure_When_Orders_Are_Not_Present()
        {
            _moqEmployeeService.Setup(m => m.ValidateEmployee(It.IsAny<string>())).Returns(Task.FromResult(GetValidEmployeeResponse()));
            _moqOrderDbContext.Setup(m => m.GetRecentEmployeeOrders(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new EmployeeOrderResponse()));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var employeeOrderServiceObject = new OrderService(_moqVendorOrderDbContext.Object, _moqOrderDbContext.Object, _moqTokenProvider.Object, _moqLogManager.Object, _moqHttpContextAccessor.Object, _moqEmployeeService.Object, _moqVendorService.Object, _moqMailService.Object, _moqEmployeeBulkOrderDbContext.Object);
            var resultant = await employeeOrderServiceObject.GetEmployeeRecentOrders(1, 1);
            Assert.Equal(Status.Failure, resultant.Status);
        }
        public GetEmployeeResponse GetValidEmployeeResponse()
        {
            return new GetEmployeeResponse()
            {
                Employee = new Employee()
                {
                    Id = "1126",
                    Firstname = "abc",
                    Lastname = "efg"
                },
                Error=null,
                Status=Status.Success
            };
        }
        public EmployeeOrder GetValidEmployeeOrder()
        {
            return new EmployeeOrder()
            {
                Employee = new Employee()
                {
                    Id = "1126",
                    Firstname = "abc",
                    Lastname = "efg"
                },
                EmployeeOrderDetails = new EmployeeOrderDetails()
                {
                    OrderId=1,
                    Date=DateTime.Now,
                    Shelf = new Shelf()
                    {
                        Id = '1',
                        Name = "FirstFloor",
                        Code = "A",
                        IsActive = true
                    },
                    EmployeeItemsQuantityList= new List<ItemQuantityMapping>()
                    {
                        new ItemQuantityMapping()
                            {
                                Item=new Item()
                                {
                                    Id=1
                                },
                                Quantity=2
                            }
                    }
                }
            };
        }
        public EmployeeOrderResponse GetValidEmployeeOrderResponse()
        {
            return new EmployeeOrderResponse()
            {
                EmployeeOrders = new List<EmployeeOrder>()
                {
                    GetValidEmployeeOrder()
                },
                Error = null,
                Status = Status.Success,
                PagingInfo = new PagingInfo()
            };
        }
    }
}