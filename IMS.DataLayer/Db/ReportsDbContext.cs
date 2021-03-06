using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IMS.Entities.Interfaces;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace IMS.DataLayer.Db
{
    public class ReportsDbContext : IReportsDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public ReportsDbContext(IDbConnectionProvider dbConnectionProvider, IItemDbContext itemDbContext)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<List<DateItemConsumption>> GetItemsConsumptionReport(string startDate, string endDate)
        {
            DbDataReader reader = null;
            List<DateItemConsumption> dateItemConsumption = new List<DateItemConsumption>();
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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
                        dateItemConsumption.Add(new DateItemConsumption() { Date = Date, ItemsConsumptionCount = Convert.ToInt32(reader["ItemsCount"]) });
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
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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
                                Name = reader["Name"]?.ToString(),
                                MaxLimit = Convert.ToInt32(reader["MaximumLimit"]),
                                IsActive = (bool)reader["IsActive"],
                                ImageUrl = reader["ImageUrl"]?.ToString(),
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

        public async void GetShelfWiseOrderCountByDate(DateTime startDate, DateTime toDate, List<ShelfOrderStats> shelfOrderStats)
        {
            DbDataReader reader = null;
            List<ShelfOrderStats> dateShelfOrderMappings = shelfOrderStats;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    DateTime newToDate = toDate.AddDays(1);
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetOrderCountByDate";
                    command.Parameters.AddWithValue("@FromDate", startDate);
                    command.Parameters.AddWithValue("@ToDate", newToDate);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {

                        DateTime date = (DateTime)reader["OrderDate"];
                        string shelfName = reader["FloorName"]?.ToString();
                        int orderCount = Convert.ToInt32(reader["TotalNumberOfEntries"]);
                        var list = dateShelfOrderMappings.FindAll(obj => obj.Date.Date.Equals(date.Date));
                        list.ForEach(obj =>
                        {
                            var shelfOrderCountMappings = obj.ShelfOrderCountMappings;
                            var internalList = shelfOrderCountMappings.FindAll(innerObj => innerObj.ShelfName.Equals(shelfName));
                            internalList.ForEach(o =>
                            {
                                o.OrderCount = orderCount;
                            });
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<Dictionary<string, List<ColourCountMapping>>> GetShelfRAGStatus()
        {
            try
            {
                DbDataReader reader = null;
                var dictionaryRAG = new Dictionary<string, List<ColourCountMapping>>();
                using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spRAGStatusOfShelves";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        string locationDetail = reader["ShelfCode"]?.ToString() + ";" + reader["ShelfName"]?.ToString();
                        var colourCountMapping = new ColourCountMapping()
                        {
                            Colour = StringToEnum(reader["ShelvesRAG"]?.ToString()),
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
                using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
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
                                Colour = StringToEnum(reader["WarehouseRAG"]?.ToString()),
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

        public async Task<ItemStockStatusDto> GetStockStatus(int limit, int offset, string itemName)
        {
            ItemStockStatusDto stockStatus = new ItemStockStatusDto();
            DbDataReader reader = null;
            stockStatus.Items = new List<Item>();
            stockStatus.StockStatus = new Dictionary<int, List<StockStatus>>();
            stockStatus.PagingInfo = new PagingInfo();
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetStockStatus";
                    command.Parameters.AddWithValue("@theLimit", limit);
                    command.Parameters.AddWithValue("@theOffset", offset);
                    command.Parameters.AddWithValue("@ItemName", itemName);
                    reader = await command.ExecuteReaderAsync();
                    int itemId;
                    Colour ragColor;
                    int quantity;
                    string shelfName;
                    while (reader.Read())
                    {
                        stockStatus.PagingInfo.TotalResults = Convert.ToInt32(reader["TotalItems"]);
                        itemId = (int)reader["ItemId"];
                        if (!(stockStatus.Items.Select(x => x.Id).Distinct().Contains(itemId)))
                        {
                            stockStatus.Items.Add(Extract(reader));
                            stockStatus.StockStatus.Add(itemId, new List<StockStatus>());
                        }
                        try
                        {
                            ragColor = StringToEnum(reader["RAG"]?.ToString());
                            shelfName = reader["Location"]?.ToString();
                            quantity = Convert.ToInt32(reader["Quantity"]);
                            if (stockStatus.StockStatus.ContainsKey(itemId))
                            {
                                stockStatus.StockStatus[itemId].Add(new StockStatus() { Location = shelfName, Colour = ragColor, Quantity = quantity });
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    reader.Close();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return stockStatus;
            }
        }
        public Item Extract(DbDataReader reader)
        {
            return new Item()
            {
                Id = (int)reader["ItemId"],
                Name = reader["Name"]?.ToString(),
                MaxLimit = Convert.ToInt32(reader["MaximumLimit"]),
                IsActive = (bool)reader["IsActive"],
                ImageUrl = reader["ImageUrl"]?.ToString(),
                Rate = Convert.ToInt32(reader["Rate"]),
                ShelvesRedLimit = Convert.ToInt32(reader["ShelvesRedLimit"]),
                ShelvesAmberLimit = Convert.ToInt32(reader["ShelvesAmberLimit"]),
                WarehouseRedLimit = Convert.ToInt32(reader["WarehouseRedLimit"]),
                WarehouseAmberLimit = Convert.ToInt32(reader["WarehouseAmberLimit"])
            };
        }

        public async Task<ItemsAvailabilityResponse> GetWarehouseAvailability(string colour, int pageNumber, int pageSize)
        {
            ItemsAvailabilityResponse itemsAvailabilityResponse = new ItemsAvailabilityResponse();
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = pageNumber;
            pagingInfo.PageSize = pageSize;
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            try
            {
                var itemQuantityMappings = new List<ItemQuantityMapping>();
                using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@colour", colour.ToUpper());
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    command.Parameters.Add("@orderCount", MySqlDbType.Int32, 32);
                    command.Parameters["@orderCount"].Direction = ParameterDirection.Output;
                    command.CommandText = "spGetWarehouseItemsByColour";
                    var reader = await command.ExecuteReaderAsync();
                    itemQuantityMappings = GetItemQuantityMapping(reader);
                    reader.Close();
                    await command.ExecuteNonQueryAsync();
                    pagingInfo.TotalResults = (int)command.Parameters["@orderCount"].Value;
                };
                itemsAvailabilityResponse.ItemQuantityMappings = itemQuantityMappings;
                itemsAvailabilityResponse.pagingInfo = pagingInfo;
                return itemsAvailabilityResponse;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<ItemsAvailabilityResponse> GetShelfAvailability(int id, string colour, int pageNumber, int pageSize)
        {
            ItemsAvailabilityResponse itemsAvailabilityResponse = new ItemsAvailabilityResponse();
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = pageNumber;
            pagingInfo.PageSize = pageSize;
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            try
            {
                var itemQuantityMappings = new List<ItemQuantityMapping>();
                using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@colour", colour.ToUpper());
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    command.Parameters.Add("@orderCount", MySqlDbType.Int32, 32);
                    command.Parameters["@orderCount"].Direction = ParameterDirection.Output;
                    command.CommandText = "spGetItemsByColourAndShelfId";
                    var reader = await command.ExecuteReaderAsync();
                    itemQuantityMappings = GetItemQuantityMapping(reader);
                    reader.Close();
                    await command.ExecuteNonQueryAsync();
                    pagingInfo.TotalResults = (int)command.Parameters["@orderCount"].Value;
                };
                itemsAvailabilityResponse.ItemQuantityMappings = itemQuantityMappings;
                itemsAvailabilityResponse.pagingInfo = pagingInfo;
                return itemsAvailabilityResponse;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private List<ItemQuantityMapping> GetItemQuantityMapping(DbDataReader reader)
        {
            var itemQuantityMappings = new List<ItemQuantityMapping>();
            while (reader.Read())
            {
                var itemQuantityMapping = new ItemQuantityMapping()
                {
                    Item = new Item()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"]?.ToString(),
                        MaxLimit = Convert.ToInt32(reader["MaximumLimit"]),
                        IsActive = (bool)reader["IsActive"],
                        ImageUrl = reader["ImageUrl"]?.ToString(),
                        Rate = Convert.ToInt32(reader["Rate"]),
                        ShelvesRedLimit = Convert.ToInt32(reader["ShelvesRedLimit"]),
                        ShelvesAmberLimit = Convert.ToInt32(reader["ShelvesAmberLimit"]),
                        WarehouseRedLimit = Convert.ToInt32(reader["WarehouseRedLimit"]),
                        WarehouseAmberLimit = Convert.ToInt32(reader["WarehouseAmberLimit"]),
                    },
                    Quantity = Convert.ToInt32(reader["Quantity"])
                };
                itemQuantityMappings.Add(itemQuantityMapping);
            }
            return itemQuantityMappings;
        }

        public async Task<DateWiseItemsConsumption> GetItemsConsumptionReports(int limit, int offset, string fromDate, string toDate)
        {
            DbDataReader reader = null;
            DateWiseItemsConsumption dateWiseItemsConsumption = new DateWiseItemsConsumption();
            List<DateItemsMapping> dateItemMapping = new List<DateItemsMapping>();
            dateWiseItemsConsumption.PagingInfo = new PagingInfo();
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetItemsConsumptionReports";
                    command.Parameters.AddWithValue("@fromDate", fromDate);
                    command.Parameters.AddWithValue("@pageLimit", limit);
                    command.Parameters.AddWithValue("@pageOffset", offset);
                    command.Parameters.AddWithValue("@toDate", toDate);
                    command.Parameters.Add("@totalResults", MySqlDbType.Int32, 32);
                    command.Parameters["@totalResults"].Direction = ParameterDirection.Output;
                    reader = await command.ExecuteReaderAsync();
                    string Date = "";
                    while (reader.Read())
                    {
                        Date = reader["Date"]?.ToString().Split(" ")[0];
                        //Date = Date.Substring(6, 4) + '/' + Date.Substring(0, 2) + '/' + Date.Substring(3, 2);
                        List<ItemQuantityMapping> itemQuantityMapping = new List<ItemQuantityMapping>
                        {
                            new ItemQuantityMapping
                            {
                                Item = new Item
                                {
                                    Name = reader["Name"]?.ToString()
                                },
                                Quantity = Convert.ToInt32(reader["Quantity"])
                            }
                        };

                        dateItemMapping.Add
                         (new DateItemsMapping
                         {
                             Date = Date,
                             ItemQuantityMappings = itemQuantityMapping
                         });
                    }
                    reader.Close();
                    await command.ExecuteNonQueryAsync();
                    dateWiseItemsConsumption.PagingInfo.TotalResults = (int)command.Parameters["@totalResults"].Value;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                var result = dateItemMapping.GroupBy(obj => obj.Date,
                    obj => obj.ItemQuantityMappings.FirstOrDefault(),
                    (key, g) => new DateItemsMapping
                    {
                        Date = key,
                        ItemQuantityMappings = g.ToList()
                    }).ToList();


                dateWiseItemsConsumption.DateItemMapping = result.OrderByDescending(obj => obj.Date).ToList();

                return dateWiseItemsConsumption;
            }
        }
        public async Task<ItemConsumptionDetailsResponse> GetDateWiseItemConsumptionDetails(string startDate, string endDate, int limit, int offset)
        {
            ItemConsumptionDetailsResponse itemsAvailabilityResponse = new ItemConsumptionDetailsResponse();
            itemsAvailabilityResponse.PagingInfo = new PagingInfo();
            Dictionary<int, List<DateItemConsumption>> itemConsumption = new Dictionary<int, List<DateItemConsumption>>();
            try
            {
                using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    command.Parameters.AddWithValue("@pageLimit", limit);
                    command.Parameters.AddWithValue("@pageOffset", offset);
                    command.Parameters.Add("@x", MySqlDbType.Int32, 32);
                    command.Parameters["@x"].Direction = ParameterDirection.Output;
                    command.CommandText = "spGetItemsConsumptionDetailsInDateRange";
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    itemsAvailabilityResponse.DateWiseItemConsumptionDetails = ExtractDictionary(reader);
                    reader.Close();
                    await command.ExecuteNonQueryAsync();
                    itemsAvailabilityResponse.PagingInfo.TotalResults = (int)command.Parameters["@x"].Value;
                };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return itemsAvailabilityResponse;
        }

        private List<DateWiseItemConsumptionDetails> ExtractDictionary(DbDataReader reader)
        {
            int id;
            int totalConsumption;
            string Date;
            Dictionary<int, List<DateItemConsumption>> itemConsumption = new Dictionary<int, List<DateItemConsumption>>();
            List<Item> items = new List<Item>();
            while (reader.Read())
            {
                id = (int)reader["ItemId"];
                if (!(items.Select(x => x.Id).Distinct().Contains(id)))
                {
                    items.Add(Extract(reader));
                    itemConsumption.Add(id, new List<DateItemConsumption>());
                    totalConsumption = Convert.ToInt32(reader["Total"]);
                    Date = reader["Date"]?.ToString();
                    if (itemConsumption.ContainsKey(id))
                    {
                        itemConsumption[id].Add(new DateItemConsumption() { Date = Date, ItemsConsumptionCount = totalConsumption });
                    }
                }
                else
                {
                    totalConsumption = Convert.ToInt32(reader["Total"]);
                    Date = reader["Date"]?.ToString();
                    if (itemConsumption.ContainsKey(id))
                    {
                        itemConsumption[id].Add(new DateItemConsumption() { Date = Date, ItemsConsumptionCount = totalConsumption });
                    }
                }
            }
            return ConvertToList(itemConsumption, items); ;
        }

        public List<DateWiseItemConsumptionDetails> ConvertToList(Dictionary<int, List<DateItemConsumption>> itemConsumption, List<Item> items)
        {
            List<DateWiseItemConsumptionDetails> consumptionList = new List<DateWiseItemConsumptionDetails>();
            foreach (Item item in items)
            {
                DateWiseItemConsumptionDetails consumption = new DateWiseItemConsumptionDetails();
                consumption.Item = item;
                consumption.DateItemConsumptions = new List<DateItemConsumption>();
                List<DateItemConsumption> dateItemConsumption = new List<DateItemConsumption>();
                if (itemConsumption.ContainsKey(item.Id))
                {
                    dateItemConsumption = itemConsumption[item.Id];
                    foreach (DateItemConsumption iterator in dateItemConsumption)
                    {
                        consumption.DateItemConsumptions.Add(iterator);
                    }
                }
                consumptionList.Add(consumption);
            }
            return consumptionList;
        }

        public async Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrders(int limit, int offset, string startDate, string endDate)
        {
            DbDataReader reader = null;
            List<EmployeeBulkOrder> employeeBulkOrders = new List<EmployeeBulkOrder>();
            List<EmployeeBulkOrderDto> employeeBulkOrdersDto = new List<EmployeeBulkOrderDto>();
            EmployeeBulkOrdersResponse employeeBulkOrdersResponse = new EmployeeBulkOrdersResponse();
            int totalResuls = 0;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "spGetAllBulkOrders";
                command.Parameters.AddWithValue("@lim", limit);
                command.Parameters.AddWithValue("@off", offset);
                command.Parameters.AddWithValue("@fromDate", startDate);
                command.Parameters.AddWithValue("@toDate", endDate);
                command.Parameters.Add("@recordCount", MySqlDbType.Int32, 32);
                command.Parameters["@recordCount"].Direction = ParameterDirection.Output;
                var out_recordCount = command.Parameters["@recordCount"];
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    employeeBulkOrdersDto.Add(Reader(reader));
                }
                reader.Close();
                if (out_recordCount.Value != DBNull.Value)
                    totalResuls = Convert.ToInt32(out_recordCount.Value);
            }
            employeeBulkOrdersResponse.EmployeeBulkOrders = DtoToEntitiesBulkOrders(employeeBulkOrdersDto);
            employeeBulkOrdersResponse.PagingInfo = new PagingInfo();
            employeeBulkOrdersResponse.PagingInfo.TotalResults = totalResuls;
            return employeeBulkOrdersResponse;
        }

        private EmployeeBulkOrderDto Reader(DbDataReader reader)
        {
            return new EmployeeBulkOrderDto()
            {
                EmployeeId = reader["EmployeeId"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"]?.ToString(),
                Email = reader["Email"].ToString(),
                ContactNumber = reader["ContactNumber"]?.ToString(),
                TemporaryCardNumber = reader["TemporaryCardNumber"]?.ToString(),
                AccessCardNumber = reader["AccessCardNumber"]?.ToString(),
                EmployeeStatus = (bool)reader["EmployeeStatus"],
                BulkOrderId = (int)reader["BulkOrderId"],
                CreatedOn = (DateTime)reader["CreatedOn"],
                RequirementDate = (DateTime)reader["RequirementDate"],
                RequestStatus = reader["RequestStatus"].ToString(),
                ReasonForRequirement = reader["ReasonForRequirement"].ToString(),
                ItemId = (int)reader["ItemId"],
                ItemName = reader["ItemName"].ToString(),
                ItemQuantityOrdered = (int)reader["ItemQuantityOrdered"],
                ItemQuantityUsed = (int)reader["ItemQuantityUsed"],
                ItemIsActive = (bool)reader["ItemIsActive"],
                ItemMaxLimit = (int)reader["ItemMaxLimit"],
                ItemImageUrl = reader["ItemImageUrl"]?.ToString(),

            };
        }

        // DTO To Model Translation
        private List<EmployeeBulkOrder> DtoToEntitiesBulkOrders(List<EmployeeBulkOrderDto> employeeBulkOrdersDto)
        {
            List<EmployeeBulkOrder> employeeBulkOrders = null;
            Dictionary<int, EmployeeBulkOrder> mapping = new Dictionary<int, EmployeeBulkOrder>();
            foreach (var bulkOrderDto in employeeBulkOrdersDto)
            {
                if (mapping.ContainsKey(bulkOrderDto.BulkOrderId))
                {
                    BulkOrderItemQuantityMapping itemQtyMapping = DtoToItemQuantityMapping(bulkOrderDto);
                    mapping[bulkOrderDto.BulkOrderId].EmployeeBulkOrderDetails.ItemsQuantityList.Add(itemQtyMapping);
                }
                else
                {
                    EmployeeBulkOrder employeeBulkOrder = ToEmployeeBulkOrder(bulkOrderDto);
                    employeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList.Add(DtoToItemQuantityMapping(bulkOrderDto));
                    mapping.Add(bulkOrderDto.BulkOrderId, employeeBulkOrder);
                }
            }
            employeeBulkOrders = mapping.Values.ToList();

            if (employeeBulkOrders == null)
            {
                employeeBulkOrders = new List<EmployeeBulkOrder>();
            }
            return employeeBulkOrders;
        }

        private EmployeeBulkOrder ToEmployeeBulkOrder(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new EmployeeBulkOrder()
            {
                BulkOrderId = bulkOrderDto.BulkOrderId,
                Employee = DtoToEmployeeObject(bulkOrderDto),
                EmployeeBulkOrderDetails = DtoToEmployeeBulkOrder(bulkOrderDto)
            };
        }

        private EmployeeBulkOrderDetails DtoToEmployeeBulkOrder(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new EmployeeBulkOrderDetails()
            {
                CreatedOn = bulkOrderDto.CreatedOn,
                RequirementDate = bulkOrderDto.RequirementDate,
                ReasonForRequirement = bulkOrderDto.ReasonForRequirement,
                BulkOrderRequestStatus = StringToBulkOrderRequestStatus(bulkOrderDto.RequestStatus),
                ItemsQuantityList = new List<BulkOrderItemQuantityMapping>()
            };

        }

        private BulkOrderRequestStatus StringToBulkOrderRequestStatus(string requestStatus)
        {
            BulkOrderRequestStatus bulkOrderRequestStatus = BulkOrderRequestStatus.Pending;
            return Enum.TryParse<BulkOrderRequestStatus>(requestStatus, out BulkOrderRequestStatus status) ? status : bulkOrderRequestStatus;
        }

        private Employee DtoToEmployeeObject(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new Employee
            {
                Id = bulkOrderDto.EmployeeId,
                Firstname = bulkOrderDto.FirstName,
                Lastname = bulkOrderDto.LastName,
                Email = bulkOrderDto.Email,
                ContactNumber = bulkOrderDto.ContactNumber,
                AccessCardNumber = bulkOrderDto.AccessCardNumber,
                TemporaryCardNumber = bulkOrderDto.TemporaryCardNumber,
                IsActive = bulkOrderDto.EmployeeStatus
            };
        }

        private BulkOrderItemQuantityMapping DtoToItemQuantityMapping(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new BulkOrderItemQuantityMapping
            {
                Item = DtoToItem(bulkOrderDto),
                QuantityOrdered = bulkOrderDto.ItemQuantityOrdered,
                QuantityUsed = bulkOrderDto.ItemQuantityUsed
            };
        }

        private Item DtoToItem(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new Item
            {

                Id = bulkOrderDto.ItemId,
                Name = bulkOrderDto.ItemName,
                IsActive = bulkOrderDto.ItemIsActive,
                MaxLimit = bulkOrderDto.ItemMaxLimit,
                ImageUrl = bulkOrderDto.ItemImageUrl
            };
        }
    }
}

