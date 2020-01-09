using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;

namespace IMS.DataLayer.Db
{
    public class ItemDbContext : IItemDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public ItemDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }
        public async Task<List<Item>> GetAllItems()
        {
            DbDataReader reader = null;
            List<Item> items = new List<Item>();

            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllItems";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        items.Add(ReadValuesFromDatabase(reader));
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return items;
        }

        public async Task<Item> GetItemById(int id)
        {
            DbDataReader reader = null;
            Item item = new Item();
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetItemById";
                    command.Parameters.AddWithValue("@Id", id);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        item=ReadValuesFromDatabase(reader);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return item;
        }

        public async Task<int> AddItem(Item item)
        {
            int latestAddedItemId = 0;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddItem";
                    command.Parameters.AddWithValue("@Id", 0);
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@MaxLimit", item.MaxLimit);
                    command.Parameters.AddWithValue("@IsActive", 1);
                    command.Parameters.AddWithValue("@ImageUrl", item.ImageUrl);
                    command.Parameters.AddWithValue("@Ranking", 1);
                    command.Parameters.AddWithValue("@Rate", item.Rate);
                    command.Parameters.AddWithValue("@ShelvesRedLimit", item.ShelvesRedLimit);
                    command.Parameters.AddWithValue("@ShelvesAmberLimit", item.ShelvesAmberLimit);
                    command.Parameters.AddWithValue("@WarehouseRedLimit", item.WarehouseRedLimit);
                    command.Parameters.AddWithValue("@WarehouseAmberLimit", item.WarehouseAmberLimit);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                        latestAddedItemId = (int)reader["Id"];
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return latestAddedItemId;
        }

        public async Task<bool> Delete(int id,bool isHardDelete)
        {
            //DbDataReader reader = null;
            bool isDeleted = false;
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDelete";
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@isHardDelete", isHardDelete);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        isDeleted = true;
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return isDeleted;
            }
        }

        public async Task<Item> UpdateItem(Item updatedItem)
        {
            
            DbDataReader reader = null;
            Item item = new Item();
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateItem";
                    command.Parameters.AddWithValue("@Id", updatedItem.Id);
                    command.Parameters.AddWithValue("@Name", updatedItem.Name);
                    command.Parameters.AddWithValue("@MaximumLimit", updatedItem.MaxLimit);
                    command.Parameters.AddWithValue("@IsActive", updatedItem.IsActive);
                    command.Parameters.AddWithValue("@ImageUrl", updatedItem.ImageUrl);
                    command.Parameters.AddWithValue("@Rate", updatedItem.Rate);
                    command.Parameters.AddWithValue("@ShelvesRedLimit", updatedItem.ShelvesRedLimit);
                    command.Parameters.AddWithValue("@ShelvesAmberLimit", updatedItem.ShelvesAmberLimit);
                    command.Parameters.AddWithValue("@WarehouseRedLimit", updatedItem.WarehouseRedLimit);
                    command.Parameters.AddWithValue("@WarehouseAmberLimit", updatedItem.WarehouseAmberLimit);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        item = ReadValuesFromDatabase(reader);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return item;
            }
        }
        public Item ReadValuesFromDatabase(DbDataReader reader )
        {
            Item item = new Item();
            item.Id = (int)reader["Id"];
            item.Name = reader["Name"]?.ToString();
            item.MaxLimit = (int)reader["MaximumLimit"];
            item.IsActive = (bool)reader["IsActive"];
            item.ImageUrl = reader["ImageUrl"]?.ToString();
            item.Rate = (double)reader["Rate"];
            item.ShelvesRedLimit = (int)reader["ShelvesRedLimit"];
            item.ShelvesAmberLimit = (int)reader["ShelvesAmberLimit"];
            item.WarehouseRedLimit = (int)reader["WarehouseRedLimit"];
            item.WarehouseAmberLimit = (int)reader["WarehouseAmberLimit"];
            return item;
        }
        public async Task<bool> IsItemAlreadyDeleted(int id, bool isHardDelete)
        {
            bool isPresent = false;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsItemAlreadyDeleted";
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@isHardDelete", isHardDelete);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        isPresent = Convert.ToBoolean(reader["isPresent"]);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return isPresent;
        }
    }
}
