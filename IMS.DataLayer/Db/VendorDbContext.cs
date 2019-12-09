using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class VendorDbContext : IVendorDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public VendorDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public Vendor GetVendorById(int vendorId)
        {
            Vendor vendor= null;
            MySqlDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetVendorById";


                    command.Parameters.AddWithValue("@Id", vendorId);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        vendor = new Vendor()
                        {
                            Id = (int)reader["Id"],
                            Name = ReturnNullOrValueAccordingly(reader["Name"]),
                            ContactNumber = ReturnNullOrValueAccordingly(reader["MobileNumber"]),
                            Title = ReturnNullOrValueAccordingly(reader["Title"]),
                            Address = ReturnNullOrValueAccordingly(reader["Address"]),
                            PAN = ReturnNullOrValueAccordingly(reader["Pan"]),
                            GST = ReturnNullOrValueAccordingly(reader["Gst"]),
                            CompanyIdentificationNumber = ReturnNullOrValueAccordingly(reader["Cin"])
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return vendor;
        }
        public static string ReturnNullOrValueAccordingly(object field)
        {
            try
            {
                return (string)field;
            }
            catch
            {
                return "";
            }
        }

        public List<Vendor> GetAllVendors()
        {
            List<Vendor> vendors = new List<Vendor>();
            MySqlDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllVendors";
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Vendor vendor = new Vendor()
                        {
                            Id = (int)reader["Id"],
                            Name = ReturnNullOrValueAccordingly(reader["Name"]),
                            ContactNumber = ReturnNullOrValueAccordingly(reader["MobileNumber"]),
                            Title = ReturnNullOrValueAccordingly(reader["Title"]),
                            Address = ReturnNullOrValueAccordingly(reader["Address"]),
                            PAN = ReturnNullOrValueAccordingly(reader["Pan"]),
                            GST = ReturnNullOrValueAccordingly(reader["Gst"]),
                            CompanyIdentificationNumber = ReturnNullOrValueAccordingly(reader["Cin"])
                        };
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
    }
}
