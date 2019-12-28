using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class EmployeeDbContext : IEmployeeDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public Employee GetEmployeeById(string employeeId)
        {
            Employee employee = null;
            MySqlDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeById";
                    command.Parameters.AddWithValue("@Id", employeeId);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = ReturnNullOrValueAccordingly(reader["Id"]),
                            Email = ReturnNullOrValueAccordingly(reader["EmailId"]),
                            ContactNumber = ReturnNullOrValueAccordingly(reader["MobileNumber"]),
                            Firstname = ReturnNullOrValueAccordingly(reader["FirstName"]),
                            Lastname = ReturnNullOrValueAccordingly(reader["LastName"]),
                            TemporaryCardNumber = ReturnNullOrValueAccordingly(reader["TemporaryCardNumber"]),
                            AccessCardNumber = ReturnNullOrValueAccordingly(reader["AccessCardNumber"]),
                            IsActive = (bool)reader["IsActive"]
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return employee;
            }
        }
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employeesList = new List<Employee>();
            MySqlDataReader reader = null;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllEmployees";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        employeesList.Add(new Employee()
                        {
                            Id = reader["Id"]?.ToString(),
                            Email = reader["EmailId"]?.ToString(),
                            ContactNumber = reader["MobileNumber"]?.ToString(),
                            Firstname = reader["FirstName"]?.ToString(),
                            Lastname = reader["LastName"]?.ToString(),
                            TemporaryCardNumber = reader["TemporaryCardNumber"]?.ToString(),
                            AccessCardNumber = reader["AccessCardNumber"]?.ToString(),
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return employeesList;
        }
        public bool CreateEmployee(Employee employee)
        {
            bool isSuccess = false;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
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
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        isSuccess = true;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return isSuccess;
        }
        public static string ReturnNullOrValueAccordingly(object field)
        {
            try
            {
                return (string)field;
            }
            catch
            {
                return "";
            }
        }
    }
}
