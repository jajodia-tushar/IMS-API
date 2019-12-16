using IMS.Contracts;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS_API.Controllers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IMS.UnitTesting.ControllerTests
{
    public class EmployeeControllerTests
    {
        public Mock<IEmployeeService> _moqEmployeeService;
        public Mock<ILogManager> _moqLogManager;

        public EmployeeControllerTests()
        {
            _moqEmployeeService = new Mock<IEmployeeService>();
            _moqLogManager = new Mock<ILogManager>();
        }

        [Fact]
        public void GetEmployeeById_Should_Return_Employee_If_Employee_Id_Is_Valid()
        {
            var entityEmployeeResponse = new IMS.Entities.GetEmployeeResponse
            {
                Status = IMS.Entities.Status.Success,
                Employee = GetEntitiesTypeEmployee()
            };
            var expectedResult = new IMS.Contracts.GetEmployeeResponse
            {
                Status = IMS.Contracts.Status.Success,
                Employee = GetContractTypeEmployee()
            };
            _moqEmployeeService.Setup(m => m.ValidateEmployee("1126")).Returns(entityEmployeeResponse);
            var employeeController = new EmployeeController(_moqEmployeeService.Object, _moqLogManager.Object);
            IMS.Contracts.GetEmployeeResponse actualResult = employeeController.GetEmployeeById("1126");
            var actual = JsonConvert.SerializeObject(actualResult);
            var expected = JsonConvert.SerializeObject(expectedResult);
            Assert.Equal(actual, expected);
        }
        [Fact]
        public void GetEmployeeById_Should_Return_NullEmployee_If_Employee_Id_Is_InValid()
        {
            var entityEmployeeResponse = new IMS.Entities.GetEmployeeResponse
            {
                Status = IMS.Entities.Status.Failure,
                Error = new IMS.Entities.Error()
                {
                    ErrorCode=IMS.Core.Constants.ErrorCodes.NotFound,
                    ErrorMessage= IMS.Core.Constants.ErrorMessages.InvalidId
                },
                Employee = null
            };
            var expectedResult = new IMS.Contracts.GetEmployeeResponse
            {
                Status = IMS.Contracts.Status.Failure,
                Error = new IMS.Contracts.Error()
                {
                    ErrorCode = IMS.Core.Constants.ErrorCodes.NotFound,
                    ErrorMessage = IMS.Core.Constants.ErrorMessages.InvalidId
                },
                Employee = null
            };
            _moqEmployeeService.Setup(m => m.ValidateEmployee("9012718")).Returns(entityEmployeeResponse);
            var employeeController = new EmployeeController(_moqEmployeeService.Object, _moqLogManager.Object);
            IMS.Contracts.GetEmployeeResponse actualResult = employeeController.GetEmployeeById("9012718");
            var actual = JsonConvert.SerializeObject(actualResult);
            var expected = JsonConvert.SerializeObject(expectedResult);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void GetEmployeeById_Should_Return_NullEmployee_If_Employee_Id_Is_Null()
        {
            var entityEmployeeResponse = new IMS.Entities.GetEmployeeResponse
            {
                Status = IMS.Entities.Status.Failure,
                Error = new IMS.Entities.Error()
                {
                    ErrorCode = IMS.Core.Constants.ErrorCodes.NotFound,
                    ErrorMessage = IMS.Core.Constants.ErrorMessages.InvalidId
                },
                Employee = null
            };
            var expectedResult = new IMS.Contracts.GetEmployeeResponse
            {
                Status = IMS.Contracts.Status.Failure,
                Error = new IMS.Contracts.Error()
                {
                    ErrorCode = IMS.Core.Constants.ErrorCodes.NotFound,
                    ErrorMessage = IMS.Core.Constants.ErrorMessages.InvalidId
                },
                Employee = null
            };
            _moqEmployeeService.Setup(m => m.ValidateEmployee("")).Returns(entityEmployeeResponse);
            var employeeController = new EmployeeController(_moqEmployeeService.Object, _moqLogManager.Object);
            IMS.Contracts.GetEmployeeResponse actualResult = employeeController.GetEmployeeById("");
            var actual = JsonConvert.SerializeObject(actualResult);
            var expected = JsonConvert.SerializeObject(expectedResult);
            Assert.Equal(actual, expected);
        }


        public IMS.Entities.Employee GetEntitiesTypeEmployee()
        {
            IMS.Entities.Employee _entityTypeEmployee = new IMS.Entities.Employee()
            {
                Id = "1126",
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                ContactNumber = "1234567890",
                Email = "rrrr@gmail.com",
                AccessCardNumber = "cs234",
                TemporaryCardNumber = "123roch",
                IsActive = true
            };
            return _entityTypeEmployee;
        }
        public IMS.Contracts.Employee GetContractTypeEmployee()
        {
            IMS.Contracts.Employee _contractsTypeEmployee = new IMS.Contracts.Employee()
            {
                Id = "1126",
                Firstname = "Rochit",
                Lastname = "Aggarwal",
                ContactNumber = null,
                Email = "rrrr@gmail.com",
                AccessCardNumber = null,
                TemporaryCardNumber =null,
                IsActive = true
            };
            return _contractsTypeEmployee;
        }
    }
}
