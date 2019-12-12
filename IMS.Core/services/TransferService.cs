using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Data;
using System.Data.Common;
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
        public async Task<bool> TransferToShelves(TransferToShelvesRequest transferRequest)
        {
            bool transferToShelfStatus = false;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spTransferFromWarehouseToShelf";
                    command.Parameters.AddWithValue("@shelfItemQuantity", StringifyShelfItemQuantityList(transferRequest));
                    int rowsAffected = command.ExecuteNonQuery();
                    transferToShelfStatus = true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                return transferToShelfStatus;
            }
        }

        private object StringifyShelfItemQuantityList(TransferToShelvesRequest transferToShelvesRequest)
        {
            string shelfItemQuantityList = "";
            foreach (TransferToShelfRequest transferToShelfRequest in transferToShelvesRequest.ShelvesItemsQuantityList)
            {
                foreach (ItemQuantityMapping itemQuantityMapping in transferToShelfRequest.ItemQuantityMapping)
                {
                    shelfItemQuantityList = shelfItemQuantityList + transferToShelfRequest.Shelf.Id.ToString() + ',' + itemQuantityMapping.Item.Id.ToString() + ',' + itemQuantityMapping.Quantity + ';';
                }
            }
            return shelfItemQuantityList;
        }
    }
}
