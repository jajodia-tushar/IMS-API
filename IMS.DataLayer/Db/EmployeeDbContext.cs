using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

            }


            return employee;
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
