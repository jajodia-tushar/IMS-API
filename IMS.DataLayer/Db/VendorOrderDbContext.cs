
using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        //vendororders
        public async Task<bool> Save(VendorOrder vendorOrder)
        {
            DbDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddVendorOrder";
                    command.Parameters.AddWithValue("@vendorid", vendorOrder.Vendor.Id);
                    command.Parameters.AddWithValue("@recievedby", vendorOrder.VendorOrderDetails.RecievedBy);
                    command.Parameters.AddWithValue("@finalamount", vendorOrder.VendorOrderDetails.FinalAmount);
                    command.Parameters.AddWithValue("@recieveddate", vendorOrder.VendorOrderDetails.Date);
                    command.Parameters.AddWithValue("@invoicenumber", vendorOrder.VendorOrderDetails.InvoiceNumber);
                    command.Parameters.AddWithValue("@submittedto", vendorOrder.VendorOrderDetails.SubmittedTo);
                    command.Parameters.AddWithValue("@invoiceimageurl", vendorOrder.VendorOrderDetails.InvoiceImageUrl);
                    command.Parameters.AddWithValue("@challannumber", vendorOrder.VendorOrderDetails.ChallanNumber);
                    command.Parameters.AddWithValue("@challanimageurl", vendorOrder.VendorOrderDetails.ChallanImageUrl);
                    string listOfItemIdQuantityPrice = ConvertToString(vendorOrder.VendorOrderDetails.OrderItemDetails);
                    command.Parameters.AddWithValue("@listof_itemid_qty_price", listOfItemIdQuantityPrice);
                    reader = await command.ExecuteReaderAsync();
                    int generatedOrderId = 0;
                    if (reader.Read())
                    {
                        generatedOrderId = Convert.ToInt32(reader["generatedorderid"]);

                    }
                    vendorOrder.VendorOrderDetails.OrderId = generatedOrderId;

                    return true;
                }
                catch (MySqlException e)
                {
                    if (e.Number == (int)MySqlErrorCode.NoReferencedRow2)
                        return false;
                    throw e;
                }
            }

        }
        public async Task<bool> ApproveOrder(VendorOrder vendorOrder)
        {
            DbDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsVendorOrderApproved";
                    command.Parameters.AddWithValue("@orderid", vendorOrder.VendorOrderDetails.OrderId);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        bool isApproved = (bool)reader["IsApproved"];
                        if (isApproved)
                            throw new OrderAlreadyApprovedException();
                    }
                    else
                    {
                        throw new InvalidOrderException();
                    }
                    command.Dispose();
                    command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spApproveVendorOrder";
                    command.Parameters.AddWithValue("@orderid", vendorOrder.VendorOrderDetails.OrderId);
                    command.Parameters.AddWithValue("@vendorid", vendorOrder.Vendor.Id);
                    command.Parameters.AddWithValue("@recievedby", vendorOrder.VendorOrderDetails.RecievedBy);
                    command.Parameters.AddWithValue("@finalamount", vendorOrder.VendorOrderDetails.FinalAmount);
                    command.Parameters.AddWithValue("@taxableamount", vendorOrder.VendorOrderDetails.TaxableAmount);
                    command.Parameters.AddWithValue("@recieveddate", vendorOrder.VendorOrderDetails.Date);
                    command.Parameters.AddWithValue("@invoicenumber", vendorOrder.VendorOrderDetails.InvoiceNumber);
                    command.Parameters.AddWithValue("@submittedto", vendorOrder.VendorOrderDetails.SubmittedTo);
                    command.Parameters.AddWithValue("@invoiceimageurl", vendorOrder.VendorOrderDetails.InvoiceImageUrl);
                    command.Parameters.AddWithValue("@challannumber", vendorOrder.VendorOrderDetails.ChallanNumber);
                    command.Parameters.AddWithValue("@challanimageurl", vendorOrder.VendorOrderDetails.ChallanImageUrl);
                    string listOfItemIdQuantityPrice = ConvertToString(vendorOrder.VendorOrderDetails.OrderItemDetails);
                    command.Parameters.AddWithValue("@listof_itemid_qty_price", listOfItemIdQuantityPrice);
                    await command.ExecuteNonQueryAsync();
                    return true;


                }
                catch (Exception e)
                {

                    if (e is OrderAlreadyApprovedException)
                        throw e;
                    if (e is InvalidOrderException)
                        return false;

                    if (e is MySqlException)
                    {
                        MySqlException mySqlException = (MySqlException)e;
                        if (mySqlException.Number == (int)MySqlErrorCode.NoReferencedRow2 )
                            return false;
                    }
                    throw e;

                }

            }
        }

        private static string ConvertToString(List<ItemQuantityPriceMapping> orderItemDetails)
        {
            return string.Join(";", orderItemDetails.Select(p => p.Item.Id + "," + p.Quantity + "," + p.TotalPrice));
        }

        public VendorOrderDto Extract(DbDataReader reader)
        {
            return new VendorOrderDto
            {
                VendorId = (int)reader["VendorId"],
                VendorName = reader["VendorName"]?.ToString(),
                VendorContactNumber = reader["VendorMobileNumber"]?.ToString(),
                VendorTitle = reader["VendorTitle"]?.ToString(),
                VendorAddress = reader["VendorAddress"]?.ToString(),
                VendorPAN = reader["VendorPAN"]?.ToString(),
                VendorGST = reader["VendorGST"]?.ToString(),
                VendorCompanyIdentificationNumber = reader["VendorCompanyIdentificationNumber"]?.ToString(),
                OrderId = (int)reader["OrderId"],
                IsApproved = (bool)reader["IsApproved"],
                RecievedBy = reader["RecievedBy"]?.ToString(),
                RecievedDate = Convert.ToDateTime(reader["RecievedDate"]),
                SubmittedTo = reader["SubmittedTo"]?.ToString(),
                TaxableAmount = (double)reader["TaxableAmount"],
                InvoiceNumber = reader["InvoiceNumber"]?.ToString(),
                InvoiceImageUrl = reader["InvoiceImageUrl"]?.ToString(),
                ChallanNumber = reader["ChallanNumber"]?.ToString(),
                ChallanImageUrl = reader["ChallanImageUrl"]?.ToString(),
                ItemId = (int)reader["ItemId"],
                ItemName = reader["ItemName"]?.ToString(),
                ItemMaxLimit = (int)reader["ItemMaxLimit"],
                ItemIsActive = (bool)reader["ItemIsActive"],
                ItemImageUrl = reader["ItemImageUrl"]?.ToString(),
                Quantity = (int)reader["ItemQuantity"],
                TotalPrice = (double)reader["ItemTotalPrice"]

            };
        }


        /* 
         * Dto To Model Translation
         */
        private List<VendorOrder> ConvertToListOfVendorOrders(List<VendorOrderDto> listOfVendorOrderDtos)
        {
            List<VendorOrder> listOfVendorOrders = null;
            Dictionary<int, VendorOrder> mapping = new Dictionary<int, VendorOrder>();
            foreach (VendorOrderDto vendorOrderDto in listOfVendorOrderDtos)
            {
                if (mapping.ContainsKey(vendorOrderDto.OrderId))
                {
                    ItemQuantityPriceMapping itemQtyPrice = ConvertToItemQuantityPriceMappingObject(vendorOrderDto);
                    mapping[vendorOrderDto.OrderId].VendorOrderDetails.OrderItemDetails.Add(itemQtyPrice);
                }
                else
                {
                    VendorOrder vendorOrder = ConvertToVendorOrderObject(vendorOrderDto);
                    mapping.Add(vendorOrderDto.OrderId, vendorOrder);
                }
            }
            listOfVendorOrders = mapping.Values.ToList();

            if (listOfVendorOrders == null)
            {
                listOfVendorOrders = new List<VendorOrder>();
            }
            return listOfVendorOrders;
        }

        private VendorOrder ConvertToVendorOrderObject(VendorOrderDto vendorOrderDto)
        {
            if (vendorOrderDto != null)
                return new VendorOrder
                {
                    Vendor = ConvertToVendorObject(vendorOrderDto),
                    VendorOrderDetails = ConvertToVendorOrderDetailsObject(vendorOrderDto)
                };
            return null;
        }
        private Vendor ConvertToVendorObject(VendorOrderDto vendorOrderDto)
        {
            return new Vendor
            {
                Id = vendorOrderDto.VendorId,
                Name = vendorOrderDto.VendorName,
                Title = vendorOrderDto.VendorTitle,
                ContactNumber = vendorOrderDto.VendorContactNumber,
                Address = vendorOrderDto.VendorAddress,
                PAN = vendorOrderDto.VendorPAN,
                GST = vendorOrderDto.VendorGST,
                CompanyIdentificationNumber = vendorOrderDto.VendorCompanyIdentificationNumber
            };

        }

        private VendorOrderDetails ConvertToVendorOrderDetailsObject(VendorOrderDto vendorOrderDto)
        {

            VendorOrderDetails vendorOrderDetails =
             new VendorOrderDetails
             {
                 OrderId = vendorOrderDto.OrderId,
                 RecievedBy = vendorOrderDto.RecievedBy,
                 SubmittedTo = vendorOrderDto.SubmittedTo,
                 Date = vendorOrderDto.RecievedDate,
                 IsApproved = vendorOrderDto.IsApproved,
                 TaxableAmount = vendorOrderDto.TaxableAmount,
                 InvoiceNumber = vendorOrderDto.InvoiceNumber,
                 InvoiceImageUrl = vendorOrderDto.InvoiceImageUrl,
                 ChallanNumber = vendorOrderDto.ChallanNumber,
                 ChallanImageUrl = vendorOrderDto.ChallanImageUrl,
                 OrderItemDetails = new List<ItemQuantityPriceMapping>()

             };
            vendorOrderDetails.OrderItemDetails.Add(ConvertToItemQuantityPriceMappingObject(vendorOrderDto));
            return vendorOrderDetails;
        }



        private ItemQuantityPriceMapping ConvertToItemQuantityPriceMappingObject(VendorOrderDto vendorOrderDto)
        {
            return new ItemQuantityPriceMapping
            {
                Item = ConvertToItemObject(vendorOrderDto),
                Quantity = vendorOrderDto.Quantity,
                TotalPrice = vendorOrderDto.TotalPrice
            };
        }

        private Item ConvertToItemObject(VendorOrderDto vendorOrderDto)
        {
            return new Item
            {

                Id = vendorOrderDto.ItemId,
                Name = vendorOrderDto.ItemName,
                IsActive = vendorOrderDto.ItemIsActive,
                MaxLimit = vendorOrderDto.ItemMaxLimit,
                ImageUrl = vendorOrderDto.ItemImageUrl
            };
        }

        public async Task<List<VendorOrder>> GetVendorOrders(bool isApproved, int pageNumber, int pageSize, DateTime startDate, DateTime endDate)
        {
            DbDataReader reader = null;
            var listOfVendorOrders = new List<VendorOrder>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    int lim = pageSize;
                    int off = (pageNumber - 1) * pageSize;
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetApprovalBasedVendorOrders";
                    command.Parameters.AddWithValue("@isApproved", isApproved);
                    command.Parameters.AddWithValue("@lim", lim);
                    command.Parameters.AddWithValue("@off", off);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    reader = await command.ExecuteReaderAsync();
                    List<VendorOrderDto> listOfVendorOrderDtos = new List<VendorOrderDto>();
                    while (reader.Read())
                    {
                        VendorOrderDto vendorOrderDto = Extract(reader);
                        listOfVendorOrderDtos.Add(vendorOrderDto);

                    }
                    listOfVendorOrders = ConvertToListOfVendorOrders(listOfVendorOrderDtos);


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return listOfVendorOrders;
        }

        public async Task<List<VendorOrder>> GetVendorOrdersByVendorId(int vendorId, int pageNumber, int pageSize, DateTime startDate, DateTime endDate)
        {
            DbDataReader reader = null;
            var listOfVendorOrders = new List<VendorOrder>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    int lim = pageSize;
                    int off = (pageNumber - 1) * pageSize;
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetVendorOrdersByVendorId";
                    command.Parameters.AddWithValue("@vendorId", vendorId);
                    command.Parameters.AddWithValue("@lim", lim);
                    command.Parameters.AddWithValue("@off", off);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    reader = await command.ExecuteReaderAsync();
                    List<VendorOrderDto> listOfVendorOrderDtos = new List<VendorOrderDto>();
                    while (reader.Read())
                    {
                        VendorOrderDto vendorOrderDto = Extract(reader);
                        listOfVendorOrderDtos.Add(vendorOrderDto);

                    }
                    listOfVendorOrders = ConvertToListOfVendorOrders(listOfVendorOrderDtos);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return listOfVendorOrders;
        }
    }
}