using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using IMS.DataLayer.Interfaces;
using IMS.Entities;

namespace IMS.DataLayer.Db
{
    public class TemporaryItemDbContext : ITemporaryItemDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public TemporaryItemDbContext(IDbConnectionProvider dbConnectionProvider)
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
                            ImageUrl = (string)reader["ImageUrl"],
                            Rate = (double)reader["Rate"]
                        });
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return items;
        }
        public Item ReadValuesFromDatabase(DbDataReader reader)
        {
            Item item = new Item();
            while (reader.Read())
            {
                item.Id = (int)reader["Id"];
                item.Name = (string)reader["Name"];
                item.MaxLimit = (int)reader["MaximumLimit"];
                item.IsActive = (bool)reader["IsActive"];
                item.ImageUrl = (string)reader["ImageUrl"];
                item.Rate = (double)reader["Rate"];
            }
            return item;
        }
    }
}
