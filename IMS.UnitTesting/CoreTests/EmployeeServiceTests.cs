using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class EmployeeServiceTests
    {
        public Mock<IEmployeeDbContext> _moqEmployeeDbContext;
        public EmployeeServiceTests()
        {
            _moqEmployeeDbContext = new Mock<IEmployeeDbContext>();
        }

        [Fact]
        public void Returns_Errors_And_Status_Failure_When_Id_Is_Null()
        {
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object);
            var response = employeeService.ValidateEmployee(0);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public void Returns_Errors_And_Status_Failure_When_Id_Is_Wrong()
        {
            Employee employee = null;
            _moqEmployeeDbContext.Setup(e => e.getEmployeeById(It.Is<int>(i => i.Equals(1134)))).Returns(employee);
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object);
            var response = employeeService.ValidateEmployee(1134);
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidId, response.Error.ErrorMessage);
            Assert.Null(response.Employee);
        }

        [Fact]
        public void Returns_Employee_And_Status_Success_Login_Service_When_Id_Is_Valid()
        {
            Employee employee = GetEmployeeDetails();
            _moqEmployeeDbContext.Setup(e => e.getEmployeeById(It.Is<int>(i => i.Equals(1126)))).Returns(employee);
            var employeeService = new EmployeeService(_moqEmployeeDbContext.Object);
            var response = employeeService.ValidateEmployee(1126);
            var actual = JsonConvert.SerializeObject(employee);
            var expected = JsonConvert.SerializeObject(response.Employee);
            Assert.Equal(actual, expected);
        }
        private Employee GetEmployeeDetails()
        {
            return new Employee()
            {
                Id = 1126,
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                Email = "rochitaggarwal54@gmail.com",
                ContactNumber = "",
                TCardNumber= "",
                AccessCardNumber = "",
                IsActive = true
            };
        }
    }
}
