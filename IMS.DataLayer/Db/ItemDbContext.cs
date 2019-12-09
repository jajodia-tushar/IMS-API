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

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
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
                        items.Add(new Item()
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            MaxLimit = (int)reader["MaximumLimit"],
                            IsActive = (bool)reader["IsActive"],
                            ImageUrl = (string)reader["ImageUrl"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return items;
        }

        public async Task<Item> GetItemById(int id)
        {
            DbDataReader reader = null;
            Item item = new Item();
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetItemById";
                    command.Parameters.AddWithValue("@Id", id);
                    reader = await command.ExecuteReaderAsync();
                    item = ReadValuesFromDatabase(reader);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return item;
        }

        public async Task<int> AddItem(Item itemRequest)
        {
            int latestAddedItemId = 0;
            DbDataReader reader = null;
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddItem";
                    command.Parameters.AddWithValue("@Id", 0);
                    command.Parameters.AddWithValue("@Name", itemRequest.Name);
                    command.Parameters.AddWithValue("@MaxLimit", itemRequest.MaxLimit);
                    command.Parameters.AddWithValue("@IsActive", 1);
                    command.Parameters.AddWithValue("@ImageUrl", itemRequest.ImageUrl);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                        latestAddedItemId = (int)reader["Id"];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return latestAddedItemId;
        }

        public async Task<bool> Delete(int id)
        {
            DbDataReader reader = null;
            bool isDeleted = false;
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDelete";
                    command.Parameters.AddWithValue("@Id", id);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        bool isActiveValue = (bool)reader["IsActive"];
                        if (!isActiveValue)
                            isDeleted = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return isDeleted;
            }
        }

        public async Task<Item> UpdateItem(Item itemRequest)
        {
            DbDataReader reader = null;
            Item item = new Item();
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateItem";
                    command.Parameters.AddWithValue("@Id", itemRequest.Id);
                    command.Parameters.AddWithValue("@Name", itemRequest.Name);
                    command.Parameters.AddWithValue("@MaximumLimit", itemRequest.MaxLimit);
                    command.Parameters.AddWithValue("@IsActive", itemRequest.IsActive);
                    command.Parameters.AddWithValue("@ImageUrl", itemRequest.ImageUrl);
                    reader = await command.ExecuteReaderAsync();
                    item = ReadValuesFromDatabase(reader);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return item;
            }
        }
        public Item ReadValuesFromDatabase(DbDataReader reader )
        {
            Item item = new Item();
            while (reader.Read())
            {
                item.Id = (int)reader["Id"];
                item.Name = (string)reader["Name"];
                item.MaxLimit = (int)reader["MaximumLimit"];
                item.IsActive = (bool)reader["IsActive"];
                item.ImageUrl = (string)reader["ImageUrl"];
            }
            return item;
        }
    }
}
