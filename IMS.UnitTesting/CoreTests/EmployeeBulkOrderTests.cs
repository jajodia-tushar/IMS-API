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

        //Get Order By Id
        [Fact]
        public async void Return_Failure_For_Bulk_Order_By_OrderId_Is_Invalid()
        {
            //Setup
            int orderId = -1;
            ValidateToken(true, Tokens.SuperAdmin);
            //Action
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Failure_For_Bulk_Order_By_OrderId_NotFound()
        {
            //Setup
            int orderId = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(1)).Returns(Task.FromResult((EmployeeBulkOrder)null));
            //Action
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Success_For_Bulk_Order_By_OrderId_Is_Found()
        {
            //Setup
            int orderId = 2;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(2)).Returns(Task.FromResult(new EmployeeBulkOrder() { BulkOrderId= orderId }));
            //Action
            var response = await _orderService.GetEmployeeBulkOrderById(orderId);
            //Verify
            Assert.Equal(Status.Success, response.Status);
            Assert.Equal(orderId,response.EmployeeBulkOrders[0].BulkOrderId);
        }
        //Get All Bulk Orders
        [Fact]
        public async void Return_Failure_For_Get_All_Bulk_Order_If_Pagenumber_Or_Pagesize_Are_Invalid()
        {
            //Setup
            string fromDate = "20180101";
            string toDate = "20210101";
            ValidateToken(true, Tokens.SuperAdmin);
            //Action
            var response = await _orderService.GetEmployeeBulkOrders(-1,-1,fromDate,toDate);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Failure_For_Get_All_Bulk_Order_If_Orders_Not_Present()
        {
            //Setup
            string fromDate = "20180101";
            string toDate = "20210101";
            int pagenumber = 1;
            int pagesize = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetAllEmployeeBulkOrders(pagenumber,pagesize,It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(Tuple.Create<int, List<EmployeeBulkOrder>>(0, new List<EmployeeBulkOrder>())));
            //Action
            var response = await _orderService.GetEmployeeBulkOrders(1, 1, fromDate, toDate);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
        }

        [Fact]
        public async void Return_Success_For_Get_All_Bulk_Order_If_Orders_Present()
        {
            //Setup
            string fromDate = "20180101";
            string toDate = "20210101";
            int pagenumber = 1;
            int pagesize = 1;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetAllEmployeeBulkOrders(pagenumber, pagesize, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(Tuple.Create<int, List<EmployeeBulkOrder>>(1, new List<EmployeeBulkOrder>() { new EmployeeBulkOrder { BulkOrderId = 1 } })));
            //Action
            var response = await _orderService.GetEmployeeBulkOrders(1, 1, fromDate, toDate);
            Assert.Equal(Status.Success, response.Status);
         }

        //Save Order i.e PlaceBulkOrder
        [Fact]
        public async void Return_Failure_For_SaveOrder_If_Order_Is_Invalid()
        {
            //Setup
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.SaveOrder(order)).Returns(Task.FromResult(false));
            //Action
            var response = await _orderService.PlaceEmployeeBulkOrder(order);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
        }
        [Fact]
        public async void Return_Success_For_SaveOrder_If_Order_Is_Valid()
        {
            //Setup
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.SaveOrder(order)).Returns(Task.FromResult(true));
            //Action
            var response = await _orderService.PlaceEmployeeBulkOrder(order);
            //Verify
            Assert.Equal(Status.Success, response.Status);
       
        }

        //Approve Order
        [Fact]
        public async void Return_Failure_For_ApproveOrder_If_Order_Is_negative()
        {    
            //Setup
            ApproveEmployeeBulkOrder order = GetApproveBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(-1,order);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderId, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Failure_For_ApproveOrder_If_Order_Is_NotPresent_In_Database()
        {
            //Setup
            int orderIdDifferntFromUrl = GetOrderId();
            int orderIdInBody = 2;
            ApproveEmployeeBulkOrder orderFromRequest = GetApproveBulkOrder();
            orderFromRequest.BulkOrderId = orderIdInBody;
            EmployeeBulkOrder orderFromDb = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(orderIdDifferntFromUrl)).Returns(Task.FromResult((EmployeeBulkOrder)null));
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(orderIdDifferntFromUrl, orderFromRequest);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderId, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Failure_For_ApproveOrder_If_Order_Is_Invalid()
        {
            //Setup
            int orderIdDifferntFromUrl = GetOrderId();
            int orderIdInBody = 2;
            ApproveEmployeeBulkOrder orderFromRequest = GetApproveBulkOrder();
            orderFromRequest.BulkOrderId = orderIdInBody;
            EmployeeBulkOrder orderFromDb = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(orderIdDifferntFromUrl)).Returns(Task.FromResult(orderFromDb));
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(orderIdDifferntFromUrl, orderFromRequest);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrder, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Failure_For_ApproveOrder_If_Order_Is_Not_In_Pending_State()
        {
            //Setup
            ApproveEmployeeBulkOrder orderFromRequest = GetApproveBulkOrder();           
            EmployeeBulkOrder orderFromDb = GetEmployeeBulkOrder();
            orderFromDb.EmployeeBulkOrderDetails.BulkOrderRequestStatus = BulkOrderRequestStatus.Approved;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(orderFromDb));
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(GetOrderId(), orderFromRequest);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderToApprove, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Failure_For_ApproveOrder_If_Items_Are_Not_AVailable()
        {
            //Setup
            ApproveEmployeeBulkOrder orderFromRequest = GetApproveBulkOrder();
            EmployeeBulkOrder orderFromDb = GetEmployeeBulkOrder();
             ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(orderFromDb));
            _moqEmployeeBulkOrderDbContext.Setup(m => m.ApproveOrder(orderFromRequest)).Returns(Task.FromResult(false));
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(GetOrderId(), orderFromRequest);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode); 
            Assert.Equal(Constants.ErrorMessages.ItemsUnavailability, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Success_For_ApproveOrder_If_Order_is_Valid()
        {
            //Setup
            ApproveEmployeeBulkOrder orderFromRequest = GetApproveBulkOrder();
            EmployeeBulkOrder orderFromDb = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(orderFromDb));
            _moqEmployeeBulkOrderDbContext.Setup(m => m.ApproveOrder(orderFromRequest)).Returns(Task.FromResult(true));
            //Action
            var response = await _orderService.ApproveEmployeeBulkOrder(GetOrderId(), orderFromRequest);
            //Verify
            Assert.Equal(Status.Success, response.Status);
           

        }

        //Reject Bulk Order
        [Fact]
        public async void Return_Failure_For_RejectOrder_If_Order_Not_Found()
        {
            //Setup
             ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult((EmployeeBulkOrder)null));
            //Action
            var response = await _orderService.RejectEmployeeBulkOrder(GetOrderId(),"Demo");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.OrderNotFound, response.Error.ErrorMessage);

        }
        
        [Fact]
        public async void Return_Failure_For_RejectOrder_If_Order_Not_In_Pending_State()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            order.EmployeeBulkOrderDetails.BulkOrderRequestStatus = BulkOrderRequestStatus.Approved;
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(order));
            //Action
            var response = await _orderService.RejectEmployeeBulkOrder(GetOrderId(), "Demo");
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderToReject, response.Error.ErrorMessage);

        }
       
        [Fact]
        public async void Return_Success_For_RejectOrder_If_OrderId_Is_Valid()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(order));
            _moqEmployeeBulkOrderDbContext.Setup(m => m.RejectOrder(GetOrderId())).Returns(Task.FromResult(true));
            //Action
            var response = await _orderService.RejectEmployeeBulkOrder(GetOrderId(), "Demo");
            //Verify
            Assert.Equal(Status.Success, response.Status);
           

        }
        //Return Items 
        [Fact]
        public async void Return_Failure_For_ReturnOrderItems_If_OrderId_Is_negative()
        {
            //Setup
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            //Action
            var response = await _orderService.ReturnOrderItems(-1, order);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderId, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Failure_For_ReturnOrderItems_If_Order_Not_Found()
        {
            //Setup
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult((EmployeeBulkOrder)null));
            //Action
            var response = await _orderService.ReturnOrderItems(GetOrderId(), order);
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.OrderNotFound, response.Error.ErrorMessage);

        }
        [Fact]
        public async void Return_Success_For_ReturnOrderItems_If_Order_Is_Valid()
        {
            //Setup
            EmployeeBulkOrder orderFromRequest = GetEmployeeBulkOrder();
            EmployeeBulkOrder orderFronDb = GetEmployeeBulkOrder();
            orderFromRequest.EmployeeBulkOrderDetails.BulkOrderRequestStatus = BulkOrderRequestStatus.Approved;
            orderFronDb.EmployeeBulkOrderDetails.BulkOrderRequestStatus = BulkOrderRequestStatus.Approved;
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(orderFronDb));
            _moqEmployeeBulkOrderDbContext.Setup(m => m.CancelOrReturnOrderItems(orderFromRequest)).Returns(Task.FromResult(GetEmployeeBulkOrder()));
            //Action
            var response = await _orderService.ReturnOrderItems(GetOrderId(), orderFronDb);
            //Verify
            Assert.Equal(Status.Success, response.Status);

        }
        //Cancel Order
        [Fact]
        public async void Return_Failure_For_CancelOrderItems_If_Order_Is_Not_Found()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult((EmployeeBulkOrder)null));
             //Action
            var response = await _orderService.CancelEmployeeBulkOrder(GetOrderId());
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderId, response.Error.ErrorMessage);
        }
        [Fact]
        public async void Return_Failure_For_CancelOrderItems_If_Order_Is_Not_In_ApprovedState()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(order));
            //Action
            var response = await _orderService.CancelEmployeeBulkOrder(GetOrderId());
            //Verify
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidOrderStatus, response.Error.ErrorMessage);
        }
        [Fact]
        public async void Return_Success_For_CancelOrderItems_If_Order_Is_Valid()
        {
            //Setup
            ValidateToken(true, Tokens.SuperAdmin);
            EmployeeBulkOrder order = GetEmployeeBulkOrder();
            order.EmployeeBulkOrderDetails.BulkOrderRequestStatus = BulkOrderRequestStatus.Approved;
            _moqEmployeeBulkOrderDbContext.Setup(m => m.GetOrderById(GetOrderId())).Returns(Task.FromResult(order));
            _moqEmployeeBulkOrderDbContext.Setup(m => m.CancelOrReturnOrderItems(order)).Returns(Task.FromResult(GetEmployeeBulkOrder()));
            //Action
            var response = await _orderService.CancelEmployeeBulkOrder(GetOrderId());
            //Verify
            Assert.Equal(Status.Success, response.Status);
           
        }


        public static ApproveEmployeeBulkOrder GetApproveBulkOrder()
        {
            ItemLocationQuantityMapping itemLocationQuantityMapping = new ItemLocationQuantityMapping
            {
                Item = new Item { Id = 1 },
                LocationQuantityMappings = new List<LocationQuantityMapping>
                {

                        new LocationQuantityMapping
                        {
                            Location="shelf1",
                            Quantity=15
                        },
                        new LocationQuantityMapping
                        {
                            Location="shelf2",
                            Quantity=15
                        }

                }
            };
            return new ApproveEmployeeBulkOrder
            {
                ItemLocationQuantityMappings = new List<ItemLocationQuantityMapping>
                {
                    itemLocationQuantityMapping

                },
                Employee = GetEmployee(),
                EmployeeBulkOrderDetails = GetEmployeeBulkOrderDetails(),
                BulkOrderId=GetOrderId()

            };
        }

        private static int GetOrderId()
        {
            return 302;
        }

        public static EmployeeBulkOrder GetEmployeeBulkOrder()
        {
            return new EmployeeBulkOrder
            {
                BulkOrderId=GetOrderId(),
                Employee = GetEmployee(),
                EmployeeBulkOrderDetails = GetEmployeeBulkOrderDetails()
            };
        }

        private static EmployeeBulkOrderDetails GetEmployeeBulkOrderDetails()
        {
            
           return new EmployeeBulkOrderDetails
            {
               ReasonForRequirement ="okr seminar",
             RequirementDate= DateTime.Now.AddDays(3),
              BulkOrderRequestStatus=BulkOrderRequestStatus.Pending,
              ItemsQuantityList=GetItemQuantityList()

           };
        }

        private static List<BulkOrderItemQuantityMapping> GetItemQuantityList()
        {
            return new List<BulkOrderItemQuantityMapping>
            {
                { new BulkOrderItemQuantityMapping { Item=new Item{Id=1},QuantityOrdered=30 ,QuantityUsed=10} }
            };
        }

        private static Employee GetEmployee()
        {
            return new Employee
            {
                Id = "1098",
                Email = "vreddy@tavisca.com",
                Firstname = "vijay",
                Lastname = "mohan",
                IsActive = true
            };
        }
    }
}
