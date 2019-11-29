using IMS.DataLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class LogDbContext : ILogDbContext
    {  
        private IConfiguration _configuration;
         public LogDbContext(IConfiguration configuration)
        {
            _configuration = configuration;  
        }
        public void Log(int userId,string status,string callType, string severity, string request, string response)
        {
            using (var connection = new MySqlConnection(_configuration["imsdb"]))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddLogData";


                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@calltype", callType);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@severity", severity);
                    command.Parameters.AddWithValue("@request", request);
                    command.Parameters.AddWithValue("@response", response);
                   



                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
