using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class AdminNotificationDbContext : IAdminNotificationDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public AdminNotificationDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<List<Notification>> GetAdminNotifications()
        {
            var notifications = new List<Notification>();
            DbDataReader reader = null;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetNotifications";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var notification = new Notification()
                        {
                            RequestId = Convert.ToInt32(reader["RequestId"]),
                            RequestType = RequestTypeStringToEnum((reader["Type"]?.ToString())),
                            RequestStatus = RequestStatusStringToEnum((reader["Status"]?.ToString())),
                            RequestedBy = reader["RequestedBy"]?.ToString(),
                            LastModified = (DateTime)reader["LastModified"]
                        };
                        notifications.Add(notification);
                    }
                    return notifications;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private RequestStatus RequestStatusStringToEnum(string requestStatus)
        {
            switch (requestStatus.ToUpper())
            {
                case "PENDING":
                    return RequestStatus.Pending;
                case "APPROVED":
                    return RequestStatus.Approved;
                case "EDITED":
                    return RequestStatus.Edited;
                case "REJECTED":
                    return RequestStatus.Rejected;
            }
            return RequestStatus.Rejected;
        }

        private RequestType RequestTypeStringToEnum(string requestType)
        {
            switch (requestType.ToUpper())
            {
                case "VENDORORDER":
                    return RequestType.VendorOrder;
                case "BULKORDER":
                    return RequestType.BulkOrder;
                case "USERMODIFICATION":
                    return RequestType.UserModification;
            }
            return RequestType.VendorOrder;
        }
    }
}
