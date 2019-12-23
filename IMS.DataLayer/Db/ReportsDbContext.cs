using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class ReportsDbContext : IReportsDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public ReportsDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<List<DateItemConsumption>> GetItemsConsumptionReport(string startDate, string endDate)
        {
            DbDataReader reader = null;
            List<DateItemConsumption> dateItemConsumption = new List<DateItemConsumption>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spItemsConsumptionStats";
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        dateItemConsumption.Add(new DateItemConsumption() { Date = reader["Date"]?.ToString(),ItemsConsumptionCount = (int)reader["ItemsCount"] });
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return dateItemConsumption;
            }
        }

        public async Task<List<ItemQuantityMapping>> GetMostConsumedItemsByDate(string startDate, string endDate, int itemsCount)
        {
            DbDataReader reader = null;
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            List<ItemQuantityMapping> itemQuantityMappings = new List<ItemQuantityMapping>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetMostConsumedItemsByDate";
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    command.Parameters.AddWithValue("@items", itemsCount);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        itemQuantityMappings.Add(new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id = Convert.ToInt32(reader["ItemId"]),
                                Name = (string)reader["Name"],
                                MaxLimit = Convert.ToInt32(reader["MaximumLimit"]),
                                IsActive = (bool)reader["IsActive"],
                                ImageUrl = (string)reader["ImageUrl"],
                                Rate = (double)reader["Rate"]
                            },
                            Quantity = Convert.ToInt32(reader["Quantity"])
                        });
                    }
                    mostConsumedItemsResponse.ItemQuantityMapping = itemQuantityMappings;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return mostConsumedItemsResponse.ItemQuantityMapping;
            }
        }

        public async Task<Dictionary<string, List<ColourCountMapping>>> GetShelfRAGStatus()
        {
            try
            {
                DbDataReader reader = null;
                var dictionaryRAG = new Dictionary<string, List<ColourCountMapping>>();
                using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spRAGStatusOfShelves";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        string locationDetail = (string)reader["ShelfCode"] + ";" + (string)reader["ShelfName"];
                        var colourCountMapping = new ColourCountMapping()
                        {
                            Colour = StringToEnum((string)reader["ShelvesRAG"]),
                            Count = Convert.ToInt32(reader["Count"])
                        };
                        if (dictionaryRAG.ContainsKey(locationDetail))
                            dictionaryRAG[locationDetail].Add(colourCountMapping);
                        else
                        {
                            var colourCountMappings = new List<ColourCountMapping>();
                            colourCountMappings.Add(colourCountMapping);
                            dictionaryRAG.Add(locationDetail, colourCountMappings);
                        }
                    }
                }
                return dictionaryRAG;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<List<ColourCountMapping>> GetWarehouseRAGStatus()
        {
            try
            {
                DbDataReader reader = null;
                var colourCountMappings = new List<ColourCountMapping>();
                using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spRAGStatusOfWarehouse";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        colourCountMappings.Add(
                            new ColourCountMapping()
                            {
                                Colour = StringToEnum((string)reader["WarehouseRAG"]),
                                Count = Convert.ToInt32(reader["Count"])
                            }
                            );
                    }
                }
                return colourCountMappings;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private Colour StringToEnum(string colour)
        {
            switch (colour.ToUpper())
            {
                case "RED":
                    return Colour.Red;
                case "AMBER":
                    return Colour.Amber;
                case "GREEN":
                    return Colour.Green;
            }
            return Colour.Green;
        }
    }
}
