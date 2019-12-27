using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class LogDbContext : ILogDbContext
    {  
        private IDbConnectionProvider _dbConnectionProvider;
         public LogDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<List<Logs>> GetLogs()
        {
            List<Logs> logsRecords = new List<Logs>();
            MySqlDataReader reader = null;
            try
            {
                using (var connection = _dbConnectionProvider.GetConnection(Databases.LOGGING))
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetLogs";

                    reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        Logs logs = new Logs()
                        {
                            LogId = (int)reader["Id"],
                            UserId=(int)reader["UserId"],
                            CallType=(string)reader["CallType"],
                            Request = (string)reader["Request"],
                            Response=(string)reader["Response"],
                            Severity =(string)reader["Severity"],
                            Status =(string)reader["Status"],
                            DateTime=(DateTime)reader["Timestamp"]

                        };
                        logsRecords.Add(logs);
                    }
                    reader.Close();

                }
            }
            catch (Exception ex)
            {

            }
            return logsRecords;
        }

        public void Log(int userId, string status, string callType, string severity, string request, string response)
        {

            try
            {
                using (var connection = _dbConnectionProvider.GetConnection(Databases.LOGGING))
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




                    command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void LogException(string callType, string request, string response, string stackTrace, string exceptionMessage, string innerException, string targetSite, string exceptionType, string severity)
        {

            try
            {
                using (var connection = _dbConnectionProvider.GetConnection(Databases.LOGGING))
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddExceptionLog";
                    command.Parameters.AddWithValue("@callType", callType);
                    command.Parameters.AddWithValue("@request", request);
                    command.Parameters.AddWithValue("@response", response);
                    command.Parameters.AddWithValue("@stackTrace", stackTrace);
                    command.Parameters.AddWithValue("@exceptionMessage", exceptionMessage);
                    command.Parameters.AddWithValue("@innerException", innerException);
                    command.Parameters.AddWithValue("@targetSite",targetSite);
                    command.Parameters.AddWithValue("@exceptionType", exceptionType);
                    command.Parameters.AddWithValue("@severity",severity);
                    command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
