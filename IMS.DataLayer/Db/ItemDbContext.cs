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
                            isActive = (bool)reader["IsActive"]
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

        public List<Item> GetItemById(int id)
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
                    command.CommandText = "spGetItemById";
                    command.Parameters.AddWithValue("@Id", id);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        _items.Add(new Item()
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            MaxLimit = (int)reader["MaximumLimit"],
                            isActive = (bool)reader["IsActive"]
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

        public List<Item> AddItem(ItemRequest itemRequest)
        {
            List<Item> _items = new List<Item>();
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
                    command.ExecuteNonQuery();
                    _items = GetAllItems();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _items;
        }

        public List<Item> Delete(int id)
        {
            List<Item> _items = new List<Item>();
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDelete";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                    _items = GetAllItems();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _items;
            }
        }

        public List<Item> UpdateItem(ItemRequest itemRequest)
        {
            List<Item> _items = new List<Item>();
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
                    command.Parameters.AddWithValue("@IsActive", itemRequest.isActive);
                    command.ExecuteNonQuery();
                    _items = GetAllItems();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _items;
            }
        }

        
    }
}
