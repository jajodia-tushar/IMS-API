using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class EmployeeOrderDbContext : IEmployeeOrderDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeOrderDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<EmployeeOrder> AddEmployeeOrder(EmployeeOrder employeeOrder)
        {
            DbDataReader reader = null;

            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddEmployeeOrder";
                    command.Parameters.AddWithValue("@EmployeeId", employeeOrder.Employee.Id);
                    command.Parameters.AddWithValue("@ShelfId", employeeOrder.EmployeeOrderDetails.Shelf.Id);
                    command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    command.Parameters.AddWithValue("@itemswithqty", ConvertToString(employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList));
                    reader = await command.ExecuteReaderAsync();
                    employeeOrder.EmployeeOrderDetails.Date = DateTime.Now;

                    int generatedOrderId = 0;
                    while (reader.Read())
                    {
                        generatedOrderId = Convert.ToInt32(reader["generatedorderid"]);

                    }
                    reader.Close();
                    employeeOrder.EmployeeOrderDetails.OrderId = generatedOrderId;
                }
                catch (Exception ex)
                {
                    employeeOrder = null;
                }
            }
            return employeeOrder;
        }
        public async Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId, int limit, int offset, string startDate, string endDate)
        {
            DbDataReader reader1 = null;
            EmployeeOrderResponse employeeOrderResponse = new EmployeeOrderResponse();
            employeeOrderResponse.EmployeeOrders = new List<EmployeeOrder>();
            employeeOrderResponse.PagingInfo = new PagingInfo();
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeOrders";
                    command.Parameters.AddWithValue("@Id", employeeId);
                    command.Parameters.AddWithValue("@lim",limit);
                    command.Parameters.AddWithValue("@off", offset);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    reader1 = await command.ExecuteReaderAsync();
                    while (reader1.Read())
                    {
                        employeeOrderResponse.EmployeeOrders.Add(new EmployeeOrder()
                        {
                            Employee = ExtractEmployee(reader1),
                            EmployeeOrderDetails = new EmployeeOrderDetails()
                            {
                                OrderId = (int)reader1["Id"],
                                Date = (DateTime)reader1["Date"],
                                Shelf = new Shelf()
                                {
                                    Id = (Int32)reader1["ShelfId"],
                                    Name = reader1["ShelfName"]?.ToString(),
                                    Code = reader1["ShelfCode"]?.ToString(),
                                    IsActive = (bool)reader1["ShelfStatus"]
                                }
                            }
                        });
                        employeeOrderResponse.PagingInfo.TotalResults = Convert.ToInt32(reader1["TotalResults"]);
                    }
                    reader1.Close();
                    foreach (EmployeeOrder currentOrder in employeeOrderResponse.EmployeeOrders)
                    {
                        currentOrder.EmployeeOrderDetails.EmployeeItemsQuantityList = new List<ItemQuantityMapping>();
                        var command2 = connection.CreateCommand();
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.CommandText = "spGetEmployeeOrderDetailsByOrderId";
                        command2.Parameters.AddWithValue("@Id", currentOrder.EmployeeOrderDetails.OrderId);
                        reader1 = await command2.ExecuteReaderAsync();
                        while (reader1.Read())
                        {
                            currentOrder.EmployeeOrderDetails.EmployeeItemsQuantityList.Add(new ItemQuantityMapping
                            {
                                Item = new Item()
                                {
                                    Name = reader1["Name"].ToString(),
                                    Id = (int)reader1["ItemId"],
                                    IsActive = (bool)reader1["ItemStatus"],
                                    MaxLimit = (int)reader1["ItemMaximumLimit"]
                                },
                                Quantity = (int)reader1["Quantity"]
                            });
                        }
                        reader1.Close();
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return employeeOrderResponse;
        }
        private string ConvertToString(List<ItemQuantityMapping> ItemsQuantityList)
        {
            string itemqty = string.Join(";", ItemsQuantityList.Select(p => p.Item.Id + "," + p.Quantity));
            return itemqty;
        }
        public async Task<EmployeeRecentOrderResponse> GetRecentEmployeeOrders(int pageSize, int pageNumber)
        {
            DbDataReader reader = null;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = pageNumber;
            pagingInfo.PageSize = pageSize;
            EmployeeRecentOrderResponse employeeRecentOrderResponse = new EmployeeRecentOrderResponse();
            List<RecentEmployeeOrderDto> recentEmployeeOrderDtos = new List<RecentEmployeeOrderDto>();
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeRecentOrders";
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    command.Parameters.Add("@orderCount", MySqlDbType.Int32, 32);
                    command.Parameters["@orderCount"].Direction = ParameterDirection.Output;
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        RecentEmployeeOrderDto employeeOrder = Extract(reader);
                        recentEmployeeOrderDtos.Add(employeeOrder);
                    }
                    reader.Close();

                    command.ExecuteNonQuery();
                    pagingInfo.TotalResults = (int)command.Parameters["@orderCount"].Value;
                    employeeRecentOrderResponse.EmployeeRecentOrders = GetListOfEmployeeRecentOrder(recentEmployeeOrderDtos);
                    employeeRecentOrderResponse.PagingInfo = pagingInfo;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return employeeRecentOrderResponse;
        }
        private static List<EmployeeRecentOrder> GetListOfEmployeeRecentOrder(List<RecentEmployeeOrderDto> recentEmployeeOrderDtos)
        {
            List<EmployeeRecentOrder> listOfEmployeeRecentOrders = null;
            Dictionary<int, EmployeeRecentOrder> mapping = new Dictionary<int, EmployeeRecentOrder>();
            foreach (RecentEmployeeOrderDto order in recentEmployeeOrderDtos)
            {
                if (mapping.ContainsKey(order.OrderId))
                {
                    ItemQuantityMapping itemQuantityMapping = ConvertToItemQuantityMappingObject(order);
                    mapping[order.OrderId].EmployeeOrder.EmployeeItemsQuantityList.Add(itemQuantityMapping);
                }
                else
                {
                    EmployeeRecentOrder employeeOrder = ConvertToEmployeeRecentOrderObject(order);
                    mapping.Add(order.OrderId, employeeOrder);
                }
            }
            listOfEmployeeRecentOrders = mapping.Values.ToList();
            if (listOfEmployeeRecentOrders == null)
            {
                return new List<EmployeeRecentOrder>();
            }
            return listOfEmployeeRecentOrders;
        }
        public static ItemQuantityMapping ConvertToItemQuantityMappingObject(RecentEmployeeOrderDto order)
        {
            return new ItemQuantityMapping
            {
                Item = new Item
                {
                    Id = order.ItemId,
                    Name = order.ItemName,
                    IsActive = order.ItemStatus
                },
                Quantity = order.ItemQuantity
            };
        }
        private static EmployeeRecentOrder ConvertToEmployeeRecentOrderObject(RecentEmployeeOrderDto employeeOrderDto)
        {
            if (employeeOrderDto != null)
            {
                return new EmployeeRecentOrder
                {
                    Employee = ConvertToEmployeeObject(employeeOrderDto),
                    EmployeeOrder = ConvertToEmployeeOrderDetailsObject(employeeOrderDto)
                };
            }
            return null;
        }

        private static EmployeeOrderDetails ConvertToEmployeeOrderDetailsObject(RecentEmployeeOrderDto employeeOrderDto)
        {
            EmployeeOrderDetails employeeOrderDetails = new EmployeeOrderDetails
            {
                OrderId = employeeOrderDto.OrderId,
                Date = employeeOrderDto.EmployeeOrderDate,
                Shelf = ConvertToShelfObject(employeeOrderDto),
                EmployeeItemsQuantityList = new List<ItemQuantityMapping>()
            };
            employeeOrderDetails.EmployeeItemsQuantityList.Add(ConvertToItemQuantityMappingObject(employeeOrderDto));
            return employeeOrderDetails;
        }

        private static Shelf ConvertToShelfObject(RecentEmployeeOrderDto employeeOrderDto)
        {
            return new Shelf
            {
                Id = employeeOrderDto.ShelfId,
                Name = employeeOrderDto.ShelfName,
                IsActive = employeeOrderDto.ShelfStatus,
                Code = employeeOrderDto.ShelfCode
            };
        }

        private static Employee ConvertToEmployeeObject(RecentEmployeeOrderDto employeeOrderDto)
        {
            return new Employee
            {
                Id = employeeOrderDto.EmployeeId,
                Firstname = employeeOrderDto.FirstName,
                Lastname = employeeOrderDto.LastName,
                Email = employeeOrderDto.Email,
                ContactNumber = employeeOrderDto.ContactNumber,
                AccessCardNumber = employeeOrderDto.AccessCardNumber,
                TemporaryCardNumber = employeeOrderDto.TemporaryCardNumber,
                IsActive = employeeOrderDto.EmployeeStatus
            };
        }

        private RecentEmployeeOrderDto Extract(DbDataReader reader)
        {
            return new RecentEmployeeOrderDto
            {
                EmployeeId = (string)reader["EmployeeId"],
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                Email = reader["EmailId"]?.ToString(),
                ContactNumber = reader["MobileNumber"]?.ToString(),
                TemporaryCardNumber = reader["TemporaryCardNumber"]?.ToString(),
                AccessCardNumber = reader["AccessCardNumber"]?.ToString(),
                EmployeeStatus = (bool)reader["IsActive"],
                OrderId = (int)reader["EmployeeOrderId"],
                EmployeeOrderDate = (DateTime)reader["EmployeeOrderDate"],
                ItemQuantity = (int)reader["ItemQuantity"],
                ItemId = (int)reader["ItemId"],
                ItemName = reader["ItemName"]?.ToString(),
                ItemStatus = (bool)reader["ItemStatus"],
                ShelfId = (int)reader["ShelfId"],
                ShelfName = reader["ShelfName"]?.ToString(),
                ShelfStatus = (bool)reader["ShelfStatus"],
                ShelfCode = reader["ShelfCode"]?.ToString()
            };
        }
        private static ItemQuantityMapping ExtractItemQuantityList(MySqlDataReader reader)
        {
            return new ItemQuantityMapping()
            {
                Item = new Item()
                {
                    Name = reader["Name"]?.ToString(),
                    Id = (int)reader["ItemId"],
                    IsActive = (bool)reader["ItemStatus"],
                    MaxLimit = (int)reader["ItemMaximumLimit"],
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    Rate = (double)reader["Rate"]
                },
                Quantity = (int)reader["Quantity"]
            };
        }
        private static EmployeeOrderDetails ExtractEmployeeOrderDetails(MySqlDataReader reader)
        {
            return new EmployeeOrderDetails()
            {
                OrderId = (int)reader["Id"],
                Date = (DateTime)reader["Date"],
                Shelf = new Shelf()
                {
                    Id = (Int32)reader["ShelfId"],
                    Name = reader["ShelfName"]?.ToString(),
                    Code = reader["ShelfCode"]?.ToString(),
                    IsActive = (bool)reader["ShelfStatus"]
                }
            };
        }
        public Employee ExtractEmployee(DbDataReader reader)
        {
            return new Employee()
            {
                Id = reader["EmployeeId"]?.ToString(),
                Email = reader["EmailId"]?.ToString(),
                ContactNumber = reader["MobileNumber"]?.ToString(),
                Firstname = reader["FirstName"]?.ToString(),
                Lastname = reader["LastName"]?.ToString(),
                TemporaryCardNumber = reader["TemporaryCardNumber"]?.ToString(),
                AccessCardNumber = reader["AccessCardNumber"]?.ToString(),
                IsActive = (bool)reader["IsActive"]
            };
        }
    }
}