using IMS.DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class AuditLogDbContext : IAuditLogsDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public AuditLogDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }
        public async void AddAuditLogs(string username, string action, string details, string performedOn, string remarks)
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
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
    }
}
