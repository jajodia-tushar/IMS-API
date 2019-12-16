
﻿using IMS.DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class VendorOrderDbContext : IVendorOrderDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public VendorOrderDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<bool> Delete(int orderId)
        {
            bool isDeleted = false;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDeleteVendorOrder";
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                        isDeleted = true;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return isDeleted;
            }
        }
    }
}