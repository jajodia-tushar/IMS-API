using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeDbContext employeeDbContext;
        private ILogManager _logger;
        public EmployeeService(IEmployeeDbContext employeeDbContext, ILogManager logger)
        {
            this.employeeDbContext = employeeDbContext;
            this._logger = logger;
        }

        public EmployeeResponse AddEmployeesFromCsvFile(string filePath)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            try
            {
                var employeesData = File.ReadAllLines(filePath);
                List<Employee> employeesList = ReadEmployeesDataFromCsv(employeesData);
                string validationResponse = ValidateEmployeeData(employeesList);
                if(String.IsNullOrEmpty(validationResponse))
                {
                    bool isSuccess = employeeDbContext.AddEmployee(employeesList);
                    if (isSuccess)
                    {
                        employeeResponse.Employees = employeeDbContext.GetAllEmployees();
                        employeeResponse.Status = Status.Success;
                    }
                    else
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict,Constants.ErrorMessages.EmployeeDataNotAdded);
                    }
                }
                else
                {
                    employeeResponse.Status = Status.Failure;
                    employeeResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = validationResponse
                    };
                }
            }
            catch(Exception exception)
            {
                employeeResponse.Status = Status.Failure;
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "AddEmployeesFromCsvFile", Severity.High, filePath, employeeResponse); }).Start();
            }
            return employeeResponse;
        }
        
        public GetEmployeeResponse ValidateEmployee(string employeeId)
        {
            GetEmployeeResponse employeeValidationResponse = new GetEmployeeResponse();
            try
            {
                if (String.IsNullOrEmpty(employeeId))
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };
                    return employeeValidationResponse;
                }
                Employee employee = employeeDbContext.GetEmployeeById(employeeId);
                if (employee != null)
                {
                    employeeValidationResponse.Status = Status.Success;
                    employeeValidationResponse.Employee = employee;

                }
                else
                {
                    employeeValidationResponse.Status = Status.Failure;
                    employeeValidationResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidId
                    };

                }
            }
            catch (Exception exception)
            {
                employeeValidationResponse.Status = Status.Failure;
                employeeValidationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "ValidateEmployee", Severity.Critical, employeeId, employeeValidationResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeValidationResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeValidationResponse, "Validating employee", employeeValidationResponse.Status, severity, -1); }).Start();
            }
            return employeeValidationResponse;
        }
        private List<Employee> ReadEmployeesDataFromCsv(string[] employeesData)
        {
            List<Employee> employeesList = new List<Employee>();
            foreach (var employeeData in employeesData)
            {
                var data = employeeData.Split(',');
                Employee employee = new Employee();
                employee.Id = data[0];
                employee.Firstname = data[1];
                employee.Lastname = data[2];
                employee.Email = data[3];
                employee.ContactNumber = data[4];
                employee.TemporaryCardNumber = data[5];
                employee.AccessCardNumber = data[6];
                employee.IsActive = Boolean.Parse(data[7]);
                employeesList.Add(employee);
            }
            return employeesList;
        }
        private String ValidateEmployeeData(List<Employee> employeesList)
        {
            string validationResponse = null;
            List<Employee> employeesListFromDb = employeeDbContext.GetAllEmployees();
            List<String> employeesIdFromDb = employeesListFromDb.Select(o => o.Id).Distinct().ToList();
            List<String> employeesContactNumberFromDb = employeesListFromDb.Select(o => o.ContactNumber).Distinct().ToList();
            List<String> employeesEmailFromDb = employeesListFromDb.Select(o => o.Email).Distinct().ToList();
            foreach (var employee in employeesList)
            {
                if (employeesIdFromDb.Contains(employee.Id) || employeesEmailFromDb.Contains(employee.Email) || employeesContactNumberFromDb.Contains(employee.ContactNumber))
                {
                    validationResponse = validationResponse + employee.Id + " ";
                }
            }
            if (validationResponse != null)
            {
                validationResponse = "Duplicate Entries for Employee Id " + validationResponse;
            }
            return validationResponse;
        }
    }
}
