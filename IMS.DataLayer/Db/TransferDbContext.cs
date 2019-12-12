using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class TransferDbContext : ITransferDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public TransferDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<Status> TransferToShelves(TransferToShelvesRequest transferToShelvesRequest)
        {
            Status transferToShelfStatus = Status.Failure;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spTransferFromWarehouseToShelf";


                    command.Parameters.AddWithValue("@shelfItemQuantity", StringifyShelfItemQuantityList(transferToShelvesRequest));
                    int rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                transferToShelfStatus = Status.Success;
                return transferToShelfStatus;
            }
        }

        private object StringifyShelfItemQuantityList(TransferToShelvesRequest transferToShelvesRequest)
        {
            string shelfItemQuantityList = "";
            foreach(TransferToShelfRequest transferToShelfRequest in transferToShelvesRequest.ShelvesItemsQuantityList)
            {
                foreach(ItemQuantityMapping itemQuantityMapping in transferToShelfRequest.ItemQuantityMapping)
                {
                    shelfItemQuantityList = shelfItemQuantityList+ transferToShelfRequest.Shelf.Id.ToString() + ',' + itemQuantityMapping.Item.Id.ToString() + ',' + itemQuantityMapping.Quantity + ';';
                }
            }
            return shelfItemQuantityList;
        }

        public async Task<Status> TransferToWarehouse(int OrderId)
        {
            throw new NotImplementedException();
        }
    }
}
