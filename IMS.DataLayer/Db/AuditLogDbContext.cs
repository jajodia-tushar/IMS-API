using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class AuditLogDbContext : IAuditLogsDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public AuditLogDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }
        public async void AddAuditLogs(string username, string action, string details, string performedOn, string remarks, string className)
        {
            using (var connection = await _dbProvider.GetConnection(Databases.LOGGING))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddAuditLogs";
                    command.Parameters.AddWithValue("@userName", username);
                    command.Parameters.AddWithValue("@action", action);
                    command.Parameters.AddWithValue("@details", details);
                    command.Parameters.AddWithValue("@performedOn", performedOn);
                    command.Parameters.AddWithValue("@remarks", remarks);
                    command.Parameters.AddWithValue("@className", className);
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
        public async Task<Tuple<int, List<ActivityLogs>>> GetActivityLogs(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate)
        {
            DbDataReader reader = null;
            List<ActivityLogs> activityLogs = new List<ActivityLogs>();
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            int totalResuls = 0;
            using (var connection = await _dbProvider.GetConnection(Databases.LOGGING))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "spGetActivityLogs";
                command.Parameters.AddWithValue("@lim", limit);
                command.Parameters.AddWithValue("@off", offset);
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);
                command.Parameters.Add("@recordCount", MySqlDbType.Int32, 32);
                command.Parameters["@recordCount"].Direction = ParameterDirection.Output;
                var out_recordCount = command.Parameters["@recordCount"];

                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    activityLogs.Add(ReadValuesFromDbReader(reader));
                }
                reader.Close();
                if (out_recordCount.Value != DBNull.Value)
                    totalResuls = Convert.ToInt32(out_recordCount.Value);
            }
            return Tuple.Create<int, List<ActivityLogs>>(totalResuls, activityLogs);
        }

        private ActivityLogs ReadValuesFromDbReader(DbDataReader reader)
        {
            return new ActivityLogs()
            {
                UserName = reader["UserName"].ToString(),
                Action = reader["Action"].ToString(),
                Details = reader["Details"]?.ToString(),
                PerformedOn = reader["PerformedOn"].ToString(),
                CreatedOn = Convert.ToDateTime(reader["CreatedON"]),
                Remarks = reader["Remarks"]?.ToString()
            };
        }
    }
}
