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
    public class ActivityLogDbContext : IActivityLogDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public ActivityLogDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<Tuple<int, List<ActivityLogs>>> GetActivityLogs(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate)
        {
            DbDataReader reader = null;
            List<ActivityLogs> activityLogs = new List<ActivityLogs>();
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            int totalResuls = 0;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.LOGGING))
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
