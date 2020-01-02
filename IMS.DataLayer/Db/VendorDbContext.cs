using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
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
            Vendor vendor = null;
            DbDataReader reader = null;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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


        public async Task<VendorsResponse> GetVendors(string name, int limit, int offset)
        {
            VendorsResponse vendorsResponse = new VendorsResponse();
            vendorsResponse.Vendors = new List<Vendor>();
            vendorsResponse.PagingInfo = new PagingInfo();
            DbDataReader reader = null;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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
                        vendorsResponse.PagingInfo.TotalResults = Convert.ToInt32(reader["totalVendorsCount"]);
                        Vendor vendor = Extract(reader);
                        vendorsResponse.Vendors.Add(vendor);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return vendorsResponse;
        }

        public async Task<Vendor> UpdateVendor(Vendor vendor)
        {
            Vendor updatedVendor = null;
            DbDataReader reader = null;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateVendor";
                    command.Parameters.AddWithValue("@Id", vendor.Id);
                    command.Parameters.AddWithValue("@Name", vendor.Name);
                    command.Parameters.AddWithValue("@MobileNumber", vendor.ContactNumber);
                    command.Parameters.AddWithValue("@Address", vendor.Address);
                    command.Parameters.AddWithValue("@Pan", vendor.PAN);
                    command.Parameters.AddWithValue("@Gst", vendor.GST);
                    command.Parameters.AddWithValue("@Cin", vendor.CompanyIdentificationNumber);
                    command.Parameters.AddWithValue("@Title", vendor.Title);

                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        updatedVendor = Extract(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return updatedVendor;
        }
        public async Task<Vendor> AddVendor(Vendor vendor)
        {
            Vendor addedVendor = null;
            DbDataReader reader = null;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    VendorValueRepetitionCheck(vendor);
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddVendor";
                    command.Parameters.AddWithValue("@Name", vendor.Name);
                    command.Parameters.AddWithValue("@MobileNumber", vendor.ContactNumber);
                    command.Parameters.AddWithValue("@Address", vendor.Address);
                    command.Parameters.AddWithValue("@Pan", vendor.PAN);
                    command.Parameters.AddWithValue("@Gst", vendor.GST);
                    command.Parameters.AddWithValue("@Cin", vendor.CompanyIdentificationNumber);
                    command.Parameters.AddWithValue("@Title", vendor.Title);

                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        addedVendor = Extract(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return addedVendor;
        }

        public async Task<bool> DeleteVendor(int vendorId, bool isHardDelete)
        {
            bool isDeleted = false;
            int rowsAffected = 0;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDeleteVendor";
                    command.Parameters.AddWithValue("@VendorId", vendorId);
                    command.Parameters.AddWithValue("@isHardDelete", isHardDelete);
                    rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        isDeleted = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return isDeleted;
        }
        public bool VendorValueRepetitionCheck(Vendor vendor)
        {
            bool isRepeated = false;
            DbDataReader reader = null;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spCheckVendorDetailsRepetition";
                    command.Parameters.AddWithValue("@Name", vendor.Name);
                    command.Parameters.AddWithValue("@Mobile", vendor.ContactNumber);
                    command.Parameters.AddWithValue("@Pan", vendor.PAN);
                    command.Parameters.AddWithValue("@Gst", vendor.GST);
                    command.Parameters.AddWithValue("@Cin", vendor.CompanyIdentificationNumber);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        isRepeated = Convert.ToBoolean(reader["isrepeated"]);
                    }
                    reader.Close();
                    if (isRepeated == true)
                    {
                        throw new InValidVendorException("Data already present");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return isRepeated;
        }
    }
}