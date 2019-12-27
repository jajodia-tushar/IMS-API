using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IMS.AddEmployees
{
    public class AddEmployeesData
    {
        private IEmployeeDbContext _employeeDbContext;
        private IConfiguration _configuration;
        public AddEmployeesData(IEmployeeDbContext employeeDbContext,IConfiguration configuration)
        {
            _employeeDbContext = employeeDbContext;
            _configuration = configuration;
        }
        public void AddEmployeesFromCsvFile()
        {
            var employeesNotAddedFileLocation = @"C:\IMSEmployeesData";
            try
            {
                var employeesData = File.ReadAllLines(_configuration["Path:FilePath"]);
                List<Employee> employeesList = ReadEmployeesDataFromCsv(employeesData);
                List<string> employeesNotAdded = _employeeDbContext.AddEmployeesFromCsvFile(employeesList);
                if (!Directory.Exists(employeesNotAddedFileLocation))
                    Directory.CreateDirectory(employeesNotAddedFileLocation);
                File.WriteAllLines(Path.Combine(employeesNotAddedFileLocation, "IMSEmployeesNotAdded.txt"), employeesNotAdded);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Status :"+Status.Failure);
                Console.WriteLine("Error Message : Internal Server Error");
                Console.WriteLine(exception);
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
