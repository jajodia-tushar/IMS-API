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
        private IConfiguration _configuration;
        public EmployeeDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Employee getEmployeeById(int id)
        {
            Employee employee = null;
            MySqlDataReader reader = null;

            using (var connection = new MySqlConnection(_configuration["imsdb"]))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeById";


                    command.Parameters.AddWithValue("@Id", id);
                    reader = command.ExecuteReader();


                    if (reader.Read())
                    {
                        employee = Extract(reader);

                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


            return employee;
        }

        private Employee Extract(MySqlDataReader reader)
        {
            return new Employee()
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                ContactNumber = (string)reader["mobile_number"],
                Firstname = (string)reader["firstname"],
                Lastname = (string)reader["lastname"],
                TemporaryCardNumber = (string)reader["TCardNumber"],
                AccessCardNumber = (string)reader["AccessCardNumber"],
                IsActive=(bool)reader["IsActive"]
            };
        }
    }
}
