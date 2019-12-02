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
        private IDbConnectionProvider _dbConnectionProvider;
         public LogDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public void Log(int userId, string status, string callType, string severity, string request, string response)
        {

            try
            {
                using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddLog";


                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@calltype", callType);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@severity", severity);
                    command.Parameters.AddWithValue("@request", request);
                    command.Parameters.AddWithValue("@response", response);




                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
