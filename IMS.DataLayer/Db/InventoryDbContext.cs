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
    public class InventoryDbContext : IInventoryDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;

        public InventoryDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<List<Entities.ItemQuantityMapping>> GetShelfItemsByShelfCode(int shelfId)
        {
            DbDataReader reader = null;
            ShelfItemsResponse shelfItemsResponse = new ShelfItemsResponse();
            List<ItemQuantityMapping> itemQuantityMappingList = new List<ItemQuantityMapping>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetShelfItemsByShelfId";
                    command.Parameters.AddWithValue("@shelfId", shelfId);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        itemQuantityMappingList.Add(new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id = (int)reader["ItemId"],
                                Name = (string)reader["ItemName"],
                                MaxLimit = (int)reader["ItemMaximumLimit"],
                                IsActive = (bool)reader["ItemState"]
                            },
                            Quantity = (int)reader["ItemQuantityAtShelf"]
                        });
                    }
                    shelfItemsResponse.itemQuantityMappings = itemQuantityMappingList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return shelfItemsResponse.itemQuantityMappings;
            }
        }
    }
}
