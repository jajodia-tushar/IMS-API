using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IMS.DataLayer;
using IMS.DataLayer.Interfaces;
using IMS.Entities;

namespace IMS.EmployeeDataDumper
{
    public class EmployeesDataDbContext : IEmployeesDataDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeesDataDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public List<Employee> CreateEmployee(List<Employee> employeesList)
        {
            List<Employee> employeesNotAdded = new List<Employee>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("---- Adding Employees Data In Progress ----");
                    Console.WriteLine("===========================================" + "\n");
                    foreach (var employee in employeesList)
                    {
                        try
                        {
                            var command = connection.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "spAddEmployee";
                            command.Parameters.AddWithValue("@Id", employee.Id);
                            command.Parameters.AddWithValue("@FirstName", employee.Firstname);
                            command.Parameters.AddWithValue("@LastName", employee.Lastname);
                            command.Parameters.AddWithValue("@EmailId", employee.Email);
                            command.Parameters.AddWithValue("@MobileNumber", employee.ContactNumber);
                            command.Parameters.AddWithValue("@TemporaryCardNumber", employee.TemporaryCardNumber);
                            command.Parameters.AddWithValue("@AccessCardNumber", employee.AccessCardNumber);
                            command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Data For Employee Id " + employee.Id + " not added");
                            employeesNotAdded.Add(
                                new Employee()
                                {
                                    Id = employee.Id,
                                    Firstname = employee.Firstname,
                                    Lastname = employee.Lastname,
                                    Email = employee.Email,
                                    ContactNumber = employee.ContactNumber,
                                    TemporaryCardNumber = employee.TemporaryCardNumber,
                                    AccessCardNumber = employee.AccessCardNumber,
                                    IsActive = employee.IsActive
                                });
                            continue;
                        }
                        Console.WriteLine("Data For Employee Id  " + employee.Id + " Added");
                    }
                    Console.WriteLine("\n" + "===================================================");
                    Console.WriteLine("---- Process For Adding Employees Is Completed ----");
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return employeesNotAdded;
            }
        }
    }
}
