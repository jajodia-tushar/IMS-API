using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.TokenManagement;
using Xunit;
using IMS.DataLayer.Interfaces;

namespace IMS.UnitTesting.CoreTests
{
    public class LogsServiceTests
    {
        public Mock<ILogDbContext> _moqLogDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;
        public Mock<IAuditLogsDbContext> _moqAuditLogsDbContext;
        ILogsService LogsService = null;

        public LogsServiceTests()
        {
            _moqAuditLogsDbContext = new Mock<IAuditLogsDbContext>();
            _moqLogDbContext = new Mock<ILogDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            LogsService = new LogsService(_moqAuditLogsDbContext.Object,_moqLogDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async void Return_Success_When_Return_Logs_List()
        {
            _moqLogDbContext.Setup(p => p.GetLogs()).Returns(Task.FromResult(GetLogs()));
            var resultant =await LogsService.GetLogsRecord();
            Assert.Equal(Status.Success,resultant.Status);
            Assert.Null(resultant.Error);
        }

        [Fact]
        public async void Return_Failure_When_Log_List_Not_Found()
        {
            _moqLogDbContext.Setup(p => p.GetLogs()).Returns(Task.FromResult(new List<Logs>()));
            var resultant = await LogsService.GetLogsRecord();
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.NotNull(resultant.Error);
        }
        [Fact]
        public async void Return_Success_When_Activity_Logs_List_Found()
        {
            _moqAuditLogsDbContext.Setup(p => p.GetActivityLogs(1, 2, new DateTime(2020, 02, 01), new DateTime(2020, 02, 03))).
                Returns(Task.FromResult(GetActivityLogs()));
            var resultant = await LogsService.GetActivityLogs(1,2,"20200201","20200203");
            Assert.Equal(Status.Success, resultant.Status);
            Assert.Null(resultant.Error);
        }
        [Fact]
        public async void Return_Failure_When_Activity_Logs_List_Not_Found()
        {
            Tuple<int, List<ActivityLogs>> activityLogs = new Tuple<int, List<ActivityLogs>>(2,new List<ActivityLogs>());
            _moqAuditLogsDbContext.Setup(p => p.GetActivityLogs(1, 2, new DateTime(2020, 02, 01), new DateTime(2020, 02, 03))).
                Returns(Task.FromResult(activityLogs));
            var resultant = await LogsService.GetActivityLogs(1, 2, "20200201", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.ActivityLogsNotPresent, resultant.Error.ErrorMessage);
        }
        [Fact]
        public async void Return_Failure_When_FromDate_Is_Invalid()
        {        
            var resultant = await LogsService.GetActivityLogs(1, 2, "20200203", "20200201");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidDate, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void Return_Failure_When_From_Date_Is_Null()
        {
            var resultant = await LogsService.GetActivityLogs(1, 2, "", "20200201");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidDate, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void Return_Failure_When_Given_Date_Format_Is_Invalid()
        {
            var resultant = await LogsService.GetActivityLogs(1, 2, "01022020", "20200201");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidDate, resultant.Error.ErrorMessage);
        }
        [Fact]
        public async void Return_Failure_When_PageNumber_Is_Negative()
        {
            var resultant = await LogsService.GetActivityLogs(-1, 2, "20200201", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidPagingDetails, resultant.Error.ErrorMessage);
        }
        [Fact]
        public async void Return_Failure_When_PageSize_Is_Negative()
        {
            var resultant = await LogsService.GetActivityLogs(1, -1, "20200201", "20200203");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidPagingDetails, resultant.Error.ErrorMessage);
        }

        private Tuple<int, List<ActivityLogs>> GetActivityLogs()
        {
            List<ActivityLogs> listOfActivityLogs = new List<ActivityLogs>()
            {
                new ActivityLogs()
                {
                    UserName = "Mangesh Naukarkar",
                   Action ="Update",
                   PerformedOn = "Eighth Floor",
                   Details= "Mangesh Naukarkar Updated Shelf With Shelf Code Eighth Floor",
                   CreatedOn= new DateTime(202,02,02,11,40,15)
                },
                new ActivityLogs()
                {
                   UserName = "Mangesh Naukarkar",
                   Action ="AddItem",
                   PerformedOn = "new Floor",
                   Details= "Mangesh Naukarkar Added Item new Floor",
                   CreatedOn=new DateTime(2020,02,02,11,44,07)
                }
            };
            Tuple<int, List<ActivityLogs>> activityLogs = new Tuple<int, List<ActivityLogs>>(2,listOfActivityLogs);
            return activityLogs;
        }

        private List<Logs> GetLogs()
        {
            return new List<Logs>()
            {
                new Logs()
                {
                    LogId=111519,
                    UserId = 29,
                    CallType ="GetAllShelves",
                    Request = "",
                    Response = "Id :1,Name:First Floor ,Code :A ,IsActive:true",
                    Severity = "Critical",
                    Status= "Success",
                    DateTime = DateTime.Now
                },
                new Logs()
                {
                    LogId=111520,
                    UserId = -1,
                    CallType ="ValidateEmployee",
                    Request = "C302",
                    Response = "{Employee:{Id:C302,Firstname:Dhvani,Lastname:Sheth,Email:dsheth@tavisca.com,IsActive:true},Status:Success,Error:null}",
                    Severity = "Critical",
                    Status= "Success",
                    DateTime = DateTime.Now
                }
            };
        }
    }
}
