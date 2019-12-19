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
                    string Date = "";
                    while (reader.Read())
                    {
                        Date = reader["Date"]?.ToString().Split(" ")[0];
                        Date = Date.Substring(6, 4) + '/' + Date.Substring(0, 2) + '/' + Date.Substring(3, 2);
                        dateItemConsumption.Add(new DateItemConsumption() { Date = Date,ItemsConsumptionCount = Convert.ToInt32(reader["ItemsCount"] )});
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

        public async Task<List<DateShelfOrderMapping>> GetShelfWiseOrderCountByDate(DateTime StartDate, DateTime ToDate)
        {
            MySqlDataReader reader1 = null;
            List<DateShelfOrderMapping> dateShelfOrderMappings = PopulateListWithZeroValues(StartDate,ToDate); 

            
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    DateTime NewToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day + 1);
              
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetOrderCountByDate";
                    command.Parameters.AddWithValue("@FromDate", StartDate);
                    command.Parameters.AddWithValue("@ToDate", NewToDate); 
                    reader1 = command.ExecuteReader();
                    while (reader1.Read())
                    {

                        DateTime date = (DateTime)reader1["OrderDate"];
                        string ShelfName = (string)reader1["FloorName"];
                        int TotalNumberOfOrder = Convert.ToInt32(reader1["TotalNumberOfEntries"]);
                        var list = dateShelfOrderMappings.FindAll(obj => obj.Date.Date.Equals(date.Date));
                        list.ForEach(obj =>
                        {
                            var ShelfOrderCountMappings = obj.ShelfOrderCountMappings;
                            var internalList = ShelfOrderCountMappings.FindAll(innerObj => innerObj.ShelfName.Equals(ShelfName));
                            internalList.ForEach(o =>
                            {
                                o.TotalNumberOfOrder = TotalNumberOfOrder;
                            });
                        });

                    }
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return dateShelfOrderMappings; 
        }

        private List<DateShelfOrderMapping> PopulateListWithZeroValues(DateTime startDate, DateTime toDate)
        {

            
            List<DateShelfOrderMapping> dateShelfOrderMappings = new List<DateShelfOrderMapping>();
            foreach (DateTime day in EachDay(startDate, toDate))
            {
                dateShelfOrderMappings.Add(
                    new DateShelfOrderMapping()
                    {
                        Date = day,
                        ShelfOrderCountMappings = GetList()
                    }
                );
            }

            return dateShelfOrderMappings;
        }

        private List<ShelfOrderCountMapping> GetList()
        {
            var list = new List<ShelfOrderCountMapping>();
            list.Add(new ShelfOrderCountMapping()
            {
                ShelfName = "First Floor",
                TotalNumberOfOrder = 0
            });
            list.Add(new ShelfOrderCountMapping()
            {
                ShelfName = "Sixth Floor",
                TotalNumberOfOrder = 0
            });
            return list;
        }
        private IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
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
