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
        public void AddEmployeesFromCsvFile()
        {
            Response employeesAddResponse = new Response();
            var employeesNotAddedFileLocation = @"C:\IMSEmployeesData";
            try
            {
                var employeesData = File.ReadAllLines(_configuration["Path:FilePath"]);
                List<Employee> employeesList = ReadEmployeesDataFromCsv(employeesData);
                List<string> employeesNotAdded = _employeesDataDbContext.CreateEmployee(employeesList);
                if (!Directory.Exists(employeesNotAddedFileLocation))
                    Directory.CreateDirectory(employeesNotAddedFileLocation);
                File.WriteAllLines(Path.Combine(employeesNotAddedFileLocation, "IMSEmployeesNotAdded.txt"), employeesNotAdded);
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
            return employeesList;
        }
    }

}
