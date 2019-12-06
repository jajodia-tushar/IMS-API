using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Data;

namespace IMS.DataLayer.Db
{
    public class ItemDbContext : IItemDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public ItemDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }
        public List<Item> GetAllItems()
        {
            MySqlDataReader reader = null;
            List<Item> _items = new List<Item>();

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllItems";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        _items.Add(new Item()
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            MaxLimit = (int)reader["MaximumLimit"],
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _items;
        }

        public Item GetItemById(int id)
        {
            MySqlDataReader reader = null;
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
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        item.Id = (int)reader["Id"];
                        item.Name = (string)reader["Name"];
                        item.MaxLimit = (int)reader["MaximumLimit"];
                        item.IsActive = (bool)reader["IsActive"];
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return item;
        }

        public int AddItem(ItemRequest itemRequest)
        {
            int latestAddedItemId = 0;
            MySqlDataReader reader = null;
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
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        latestAddedItemId = (int)reader["Id"];
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return latestAddedItemId;
        }

        public bool Delete(int id)
        {
            MySqlDataReader reader = null;
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
                    reader = command.ExecuteReader();
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

        public Item UpdateItem(ItemRequest itemRequest)
        {
            MySqlDataReader reader = null;
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
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        item.Id = (int)reader["Id"];
                        item.Name = (string)reader["Name"];
                        item.MaxLimit = (int)reader["MaximumLimit"];
                        item.IsActive = (bool)reader["IsActive"];
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return item;
            }
        }

        
    }
}
