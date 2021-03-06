using IMS.Core;
using IMS.EmployeeDataDumper;
using IMS.Entities;
using IMS.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.EmployeeDataDumper
{
    public class EmployeesDataInjector
    {
        private IEmployeesDataDbContext _employeesDataDbContext;
        private IConfiguration _configuration;
        private ILogManager _logger;
        public EmployeesDataInjector(IEmployeesDataDbContext employeesDataDbContext,IConfiguration configuration,ILogManager logger)
        {
            _employeesDataDbContext = employeesDataDbContext;
            _configuration = configuration;
            _logger = logger;
        }
        public async void AddEmployeesFromCsvFile()
        {
            Response employeesAddResponse = new Response();
            var employeesNotAddedFileLocation = @"C:\IMSEmployeesData\";
            var employeesNotAddedFileName = "IMSEmployeesNotAdded_"+DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")+".csv";
            try
            {
                var employeesData = File.ReadAllLines(_configuration["Path:FilePath"]);
                List<Employee> employeesList = ReadEmployeesDataFromCsv(employeesData);
                List<Employee> employeesNotAdded = await _employeesDataDbContext.CreateEmployee(employeesList);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Join(",", "Id", 
                    "First Name", 
                    "Last Name", 
                    "Email", 
                    "Contact Number", 
                    "Temporary Card Number", 
                    "Access Card Number", 
                    "IsActive"));
                for (int index = 0; index < employeesNotAdded.Count; index++)
                    sb.AppendLine(string.Join(",", employeesNotAdded[index].Id,employeesNotAdded[index].Firstname,
                        employeesNotAdded[index].Lastname,employeesNotAdded[index].Email,
                        employeesNotAdded[index].ContactNumber,
                        employeesNotAdded[index].TemporaryCardNumber,
                        employeesNotAdded[index].AccessCardNumber,
                        employeesNotAdded[index].IsActive));
                
                if (!Directory.Exists(employeesNotAddedFileLocation))
                    Directory.CreateDirectory(employeesNotAddedFileLocation);
                File.AppendAllText(Path.Combine(employeesNotAddedFileLocation, employeesNotAddedFileName),sb.ToString());
                if (employeesNotAdded.Count > 0)
                    employeesAddResponse.Status = Status.Failure;
                else
                    employeesAddResponse.Status = Status.Success;
            }
            catch (Exception exception)
            {
                employeesAddResponse.Status = Status.Failure;
                employeesAddResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError,Constants.ErrorMessages.ServerError);
                Console.WriteLine("Status: "+employeesAddResponse.Status);
                Console.WriteLine("Sorry, Internal Server Error");
                new Task(() => { _logger.LogException(exception, "AddEmployeesFromCsvFile", Severity.High, null,employeesAddResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeesAddResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(null, employeesAddResponse, "AddEmployeesFromCsvFile", employeesAddResponse.Status, severity, -1); }).Start();
            }
        }
        public List<Employee> ReadEmployeesDataFromCsv(string[] employeesData)
        {
            List<Employee> employeesList = new List<Employee>();
            try
            {
                foreach (var employeeData in employeesData)
                {
                    var data = employeeData.Split(',');
                    Employee employee = new Employee();
                    employee.Id = data[0].Trim();
                    employee.Firstname = data[1].Trim();
                    employee.Lastname = data[2].Trim();
                    employee.Email = data[3].Trim();
                    employee.ContactNumber = data[4].Trim();
                    employee.TemporaryCardNumber = data[5].Trim();
                    employee.AccessCardNumber = data[6].Trim();
                    employee.IsActive = Boolean.Parse(data[7]);
                    employeesList.Add(employee);
                }
            }
            catch(Exception exception)
            {
                throw exception;
            }
            return employeesList;
        }
    }
}
