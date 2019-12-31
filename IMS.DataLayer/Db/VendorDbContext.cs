using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db

{
    public class VendorDbContext : IVendorDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public VendorDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<Vendor> GetVendorById(int vendorId)
        {
            Vendor vendor= null;
            DbDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetVendorById";


                    command.Parameters.AddWithValue("@Id", vendorId);
                    reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                       vendor = Extract(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return vendor;
        }
        public Vendor Extract(DbDataReader reader)
        {
            return new Vendor()
            {
                Id = (int)reader["Id"],
                Name = reader["Name"]?.ToString(),
                ContactNumber = reader["MobileNumber"]?.ToString(),
                Title = reader["Title"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                PAN = reader["Pan"]?.ToString(),
                GST = reader["Gst"]?.ToString(),
                CompanyIdentificationNumber = reader["Cin"]?.ToString()
            };
        }

        public async Task<List<Vendor>> GetAllVendors()
        {
            List<Vendor> vendors = new List<Vendor>();
            DbDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllVendors";
                    reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        Vendor vendor = Extract(reader);
                        vendors.Add(vendor);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return vendors;
        }

        public async Task<int> SearchByName(string name, int limit, int offset, List<Vendor> vendors)
        {
            DbDataReader reader = null;
            int totalCountOfData = 0;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetVendorByName";
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@theLimit", limit);
                    command.Parameters.AddWithValue("@theOffset", offset);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        totalCountOfData = Convert.ToInt32(reader["totalVendorsCount"]);
                        Vendor vendor = Extract(reader);
                        vendors.Add(vendor);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return totalCountOfData;
        }
    }
}
