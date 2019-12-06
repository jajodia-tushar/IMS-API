using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class InventoryDbContext : IInventoryDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;

        public InventoryDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public ShelfItemsResponse GetShelfItemsByShelfId(int shelfId)
        {
            MySqlDataReader reader = null;
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
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        itemQuantityMappingList.Add(new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id = (int)reader["ItemId"],
                                Name = (string)reader["ItemName"],
                                MaxLimit = (int)reader["ItemMaximumLimit"],
                                isActive = (bool)reader["ItemState"]
                            },
                            Quantity = (int)reader["ItemQuantityAtShelf"]
                        });
                    }
                    Shelf shelf = GetShelfById(1);
                    shelfItemsResponse.itemQuantityMappings = itemQuantityMappingList;
                    shelfItemsResponse.shelf = shelf;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return shelfItemsResponse;
            }
        }
        public Shelf GetShelfById(int shelfId)
        {
            MySqlDataReader reader = null;
            Shelf shelf = new Shelf();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetShelfById";
                    command.Parameters.AddWithValue("@shelfId", shelfId);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        shelf.Id = (int)reader["Id"];
                        shelf.Name = (string)reader["Name"];
                        shelf.IsActive = (bool)reader["IsActive"];
                        shelf.Code = (string)reader["Code"];
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return shelf;
        }
    }
}
