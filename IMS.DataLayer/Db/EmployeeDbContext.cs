using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class EmployeeDbContext : IEmployeeDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public  async Task<Employee> GetEmployeeById(string employeeId)
        {
            Employee employee = null;
            DbDataReader reader = null;

            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeById";
                    command.Parameters.AddWithValue("@Id", employeeId);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        employee = Extract(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return employee;
            }
        }
        public async Task<EmployeeResponse> GetAllEmployees(string employeeId, string employeeName, int limit, int offset)
        {
            EmployeeResponse employeesResponse = new EmployeeResponse() ;
            employeesResponse.Employees = new List<Employee>();
            employeesResponse.PagingInfo = new PagingInfo();
            DbDataReader reader = null;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllEmployees";
                    command.Parameters.AddWithValue("@Id", employeeId);
                    command.Parameters.AddWithValue("@Name", employeeName);
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        employeesResponse.Employees.Add(Extract(reader));
                        employeesResponse.PagingInfo.TotalResults = Convert.ToInt32(reader["totalEmployeesCount"]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return employeesResponse;
        }
        public Employee Extract(DbDataReader reader)
        {
            return new Employee()
            {
                Id = reader["Id"]?.ToString(),
                Email = reader["EmailId"]?.ToString(),
                ContactNumber = reader["MobileNumber"]?.ToString(),
                Firstname = reader["FirstName"]?.ToString(),
                Lastname = reader["LastName"]?.ToString(),
                TemporaryCardNumber = reader["TemporaryCardNumber"]?.ToString(),
                AccessCardNumber = reader["AccessCardNumber"]?.ToString(),
                IsActive = (bool)reader["IsActive"]
            };
        }
        public async Task<bool> CreateEmployee(Employee employee)
        {
            bool isSuccess = false;
            using (var connection =await  _dbConnectionProvider.GetConnection(Databases.IMS))
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
                    int rowsAffected = await command.ExecuteNonQueryAsync();
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

        public async Task<bool> CheckEmpEmailAvailability(string email)
        {
            bool emailIdAlreadypresent = false;
            DbDataReader reader = null;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spEmailIdAlreadyPresent";
                    command.Parameters.AddWithValue("@emailID", email);
                    reader = await command.ExecuteReaderAsync();

                    if (reader.Read())
                    {   
                        emailIdAlreadypresent = true;
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return emailIdAlreadypresent;
        }
    }
}
