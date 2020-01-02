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
        }

        [Fact]
        public async void Returns_Errors_And_Status_Failure_When_Id_Is_Null()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object,_mockTokenProvider.Object,_mockHttpContextAccessor.Object);
            var response = await employeeService.ValidateEmployee("");
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public async void Returns_Errors_And_Status_Failure_When_Id_Is_Wrong()
        {
            Task<Employee> employee = null;
            _moqEmployeeDbContext.Setup(e => e.GetEmployeeById(It.Is<string>(i => i.Equals("1134")))).Returns(employee);
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var response =await employeeService.ValidateEmployee("1134");
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public async void Returns_Employee_And_Status_Success_Login_Service_When_Id_Is_Valid()
        {
            Employee employee = await GetEmployeeDetails();
            _moqEmployeeDbContext.Setup(e => e.GetEmployeeById(It.Is<string>(i => i.Equals("1126")))).Returns(Task.FromResult(employee));
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object, _moqILogManager.Object, _mockTokenProvider.Object, _mockHttpContextAccessor.Object);
            var response =await employeeService.ValidateEmployee("1126");
            var actual = JsonConvert.SerializeObject(employee);
            var expected = JsonConvert.SerializeObject(Task.FromResult(response.Employee));
            Assert.Equal(actual, expected);
        }
        private Task<Employee> GetEmployeeDetails()
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
    }
}
