using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class EmployeeServiceTests
    {
        public Mock<IEmployeeDbContext> _moqEmployeeDbContext;
        public Mock<ILogManager> _moqILogManager;
        public Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        public Mock<ITokenProvider> _mockTokenProvider;

        public EmployeeServiceTests()
        {
            _moqEmployeeDbContext = new Mock<IEmployeeDbContext>();
            _moqILogManager = new Mock<ILogManager>();
            _mockHttpContextAccessor= new Mock<IHttpContextAccessor> () ;
            _mockTokenProvider = new Mock<ITokenProvider> ();
            _mockTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async void ValidateEmployee_Should_Return_Error_And_Status_Failure_When_Employee_Id_Is_Null()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object,_mockTokenProvider.Object,_mockHttpContextAccessor.Object);
            var response = await employeeService.ValidateEmployee("");
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InValidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public async void ValidateEmployee_Should_Return_Error_And_Status_Failure_When_Employee_Id_Is_Not_Valid()
        {
            Employee employee = null;
            _moqEmployeeDbContext.Setup(e => e.GetEmployeeById(It.Is<string>(i => i.Equals("1134")))).Returns(Task.FromResult(employee));
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var response =await employeeService.ValidateEmployee("1134");
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InValidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public async void ValidateEmployee_Should_Returns_Success_And_Valid_Employee_When_Employee_Id_Is_Present_In_Database()
        {
            
            Employee employee = await GetEmployeeDetails();
            _moqEmployeeDbContext.Setup(e => e.GetEmployeeById(It.Is<string>(i => i.Equals("1126")))).Returns(GetEmployeeDetails());
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var response =await employeeService.ValidateEmployee("1126");
            Assert.Equal(Status.Success, response.Status);
            Assert.NotNull(response.Employee);
        }

        [Fact]
        public async void GetAllEmployees_Should_Returns_Valid_Response_When_Employees_Are_Present_In_Database()
        {
            _moqEmployeeDbContext.Setup(m => m.GetAllEmployees("some",10,0)).Returns(GetAllEmployees());
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var resultant = await employeeService.GetAllEmployees("some", 1, 10);
            Assert.Equal(Status.Success,resultant.Status);
            Assert.NotNull(resultant.Employees);
        }

        [Fact]
        public async void GetAllEmployees_Should_Return_Error_And_Status_Failure_When_Employees_Are_Not_Present_In_Database()
        {
            _moqEmployeeDbContext.Setup(m => m.GetAllEmployees("some", 10, 0)).Returns(GetEmptyEmployeeResponse());
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var resultant = await employeeService.GetAllEmployees("some", 1, 10);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Null(resultant.Employees);
        }
        [Fact]
        public async void AddEmployee_Should_Return_Error_And_Status_Failure_When_Employee_Id_Is_Null()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var resultant = await employeeService.Add(new Employee() { Id = "", Firstname = "rochit", Lastname = "Aggarwal", Email = "raggarwal@tavisca.com", AccessCardNumber = "", TemporaryCardNumber = "", ContactNumber = "", IsActive = true });
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidEmployeeDetails, resultant.Error.ErrorMessage);
            Assert.Null(resultant.Employees);
        }
        [Fact]
        public async void AddEmployee_Should_Return_Sucess_When_Employee_Details_Is_Valid()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            _moqEmployeeDbContext.Setup(m => m.EmployeeDetailsRepetitionCheck(GetEmployeeForRequest())).Returns(Task.FromResult(false));
            _moqEmployeeDbContext.Setup(m => m.CreateEmployee(It.IsAny<Employee>())).Returns(Task.FromResult("1126"));
            _moqEmployeeDbContext.Setup(m => m.GetEmployeeById("1126")).Returns(GetEmployeeDetails());
            var resultant = await employeeService.Add(GetEmployeeForRequest());
            Assert.Equal(Status.Success, resultant.Status);
        }
        [Fact]
        public async void UpdateEmployee_Should_Return_Error_When_Employee_Details_Is_Not_Valid()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var resultant = await employeeService.Update(new Employee() { Id = "", Firstname = "Rochit", Email = "raggarwal@tavisca.com", IsActive = true });
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Null(resultant.Employees);
        }
        [Fact]
        public async void Update_Employee_Should_Return_Success_When_Employee_Details_Is_Valid()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            _moqEmployeeDbContext.Setup(m => m.Update(It.IsAny<Employee>())).Returns(Task.FromResult(GetEmployeeForRequest()));
            var resultant = await employeeService.Update(GetEmployeeForRequest());
            Assert.Equal(Status.Success, resultant.Status);
            Assert.Null(resultant.Error);
            Assert.NotNull(resultant.Employees);
        }
        [Fact]
        public async void DeleteEmployee_Should_Return_Success_When_Employee_Details_Is_Valid()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            _moqEmployeeDbContext.Setup(m => m.GetEmployeeById(It.IsAny<string>())).Returns(Task.FromResult(GetEmployeeForRequest()));
            _moqEmployeeDbContext.Setup(m => m.Delete(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var resultant = await employeeService.Delete("1126", false);
            Assert.Equal(Status.Success, resultant.Status);
            Assert.Null(resultant.Error);
        }
        [Fact]
        public async void DeleteEmployee_Should_Return_Status_Failure_Employee_Id_Is_Empty()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var resultant = await employeeService.Delete("", false);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.NotNull(resultant.Error);
        }
        Task<Employee> GetEmployeeDetails()
        {
            Employee employee= new Employee()
            {
                Id = "1126",
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                Email = "rochitaggarwal54@gmail.com",
                ContactNumber = "",
                TemporaryCardNumber= "",
                AccessCardNumber = "",
                IsActive = true
            };
            return Task.FromResult(employee);
        }
        Task<EmployeeResponse> GetEmptyEmployeeResponse()
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            employeeResponse.Employees = null;
            employeeResponse.PagingInfo = null;
            employeeResponse.Status = Status.Failure;
            Error error = new Error() { ErrorCode = Constants.ErrorCodes.BadRequest, ErrorMessage = Constants.ErrorMessages.NoEmployeesPresent };
            employeeResponse.Error = error;
            return Task.FromResult(employeeResponse);
        }
        Task<EmployeeResponse> GetAllEmployees()
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            List<Employee> employees = new List<Employee>()
            {
                GetEmployeeForRequest()
            };
            PagingInfo pagingInfo = new PagingInfo()
            {
                PageNumber = 1,
                PageSize = 10,
                TotalResults = 20
            };
            employeeResponse.Employees = employees;
            employeeResponse.PagingInfo = pagingInfo;
            employeeResponse.Status = Status.Success;
            employeeResponse.Error = null;
            return Task.FromResult(employeeResponse);
        }
        private Employee GetEmployeeForRequest()
        {
            return new Employee()
            {
                Id = "1126",
                Firstname = "Varsha",
                Lastname = "Vinod",
                Email = "vvinod@gmail.com",
                ContactNumber = "4957577471",
                TemporaryCardNumber = "f454545472",
                AccessCardNumber = "h4572847457",
                IsActive = true
            };
        }
    }
}
