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

        public async Task<List<Entities.ItemQuantityMapping>> GetShelfItemsByShelfId(int shelfId)
        {
            DbDataReader reader = null;
            ShelfItemsResponse shelfItemsResponse = new ShelfItemsResponse();
            List<ItemQuantityMapping> itemQuantityMappings = new List<ItemQuantityMapping>();
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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
                        itemQuantityMappings.Add(new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id = Convert.ToInt32(reader["ItemId"]),
                                Name = reader["ItemName"]?.ToString(),
                                MaxLimit = Convert.ToInt32(reader["ItemMaximumLimit"]),
                                IsActive = (bool)reader["ItemState"]
                            },
                            Quantity = Convert.ToInt32(reader["ItemQuantityAtShelf"])
                        });
                    }
                    shelfItemsResponse.ItemQuantityMappings = itemQuantityMappings;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return shelfItemsResponse.ItemQuantityMappings;
            }
        }
    }
}
