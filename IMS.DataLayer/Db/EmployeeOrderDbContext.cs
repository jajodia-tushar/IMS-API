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
        public async Task<List<EmployeeOrderDetails>> GetOrdersByEmployeeId(string id)
        {
            DbDataReader reader1 = null;
            List<EmployeeOrderDetails> employeeOrders = new List<EmployeeOrderDetails>();
            using (var connection =await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeOrdersByEmployeeId";
                    command.Parameters.AddWithValue("@Id", id);
                    reader1 = await command.ExecuteReaderAsync();
                    while (reader1.Read())
                    {
                        employeeOrders.Add(new EmployeeOrderDetails()
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
                        });
                    }
                    reader1.Close();
                    foreach (EmployeeOrderDetails currentOrder in employeeOrders)
                    {
                        currentOrder.EmployeeItemsQuantityList = new List<ItemQuantityMapping>();
                        var command2 = connection.CreateCommand();
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.CommandText = "spGetEmployeeOrderDetailsByOrderId";
                        command2.Parameters.AddWithValue("@Id", currentOrder.OrderId);
                        reader1 = await command2.ExecuteReaderAsync();
                        while (reader1.Read())
                        {
                            currentOrder.EmployeeItemsQuantityList.Add(new ItemQuantityMapping
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
            return employeeOrders;
        }
        private string ConvertToString(List<ItemQuantityMapping> ItemsQuantityList)
        {
            string itemqty = string.Join(";", ItemsQuantityList.Select(p => p.Item.Id + "," + p.Quantity));
            return itemqty;
        }
        public async Task<EmployeeRecentOrderResponse> GetRecentEmployeeOrders(int pageSize,int pageNumber)
        {
            DbDataReader reader = null;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = pageNumber;
            pagingInfo.PageSize = pageSize;
            EmployeeRecentOrderResponse employeeRecentOrderResponse = new EmployeeRecentOrderResponse
            { 
                Status = Status.Failure
            };
            List<RecentEmployeeOrderDto> recentEmployeeOrderDtos = new List<RecentEmployeeOrderDto>();
            int limit =pageSize;
            int offset = (pageNumber - 1) *pageSize;
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
                    reader =await command.ExecuteReaderAsync();
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
            return employeeRecentOrderResponse ;
        }
        private static List<EmployeeRecentOrder> GetListOfEmployeeRecentOrder(List<RecentEmployeeOrderDto> recentEmployeeOrderDtos)
        {
            List<EmployeeRecentOrder> listOfEmployeeRecentOrders = new List<EmployeeRecentOrder>();
            Dictionary<int, EmployeeRecentOrder> mapping = new Dictionary<int, EmployeeRecentOrder>();
            foreach (RecentEmployeeOrderDto order in recentEmployeeOrderDtos)
            {
                if (mapping.ContainsKey(order.OrderId))
                {
                    ItemQuantityMapping itemQuantityMapping = new ItemQuantityMapping
                    {
                        Item = new Item
                        {
                            Id = order.ItemId,
                            Name = order.ItemName,
                            IsActive = order.ItemStatus
                        },
                        Quantity = order.ItemQuantity
                    };
                    mapping[order.OrderId].EmployeeOrder.EmployeeItemsQuantityList.Add(itemQuantityMapping);
                }
                else
                {
                    EmployeeRecentOrder employeeOrder = ConvertToEmployeeRecentOrderObject(order);
                    mapping.Add(order.OrderId, employeeOrder);
                }
            }
            listOfEmployeeRecentOrders = mapping.Values.ToList();
            bool chk = !listOfEmployeeRecentOrders.Any();
            if (chk)
            {
                return new List<EmployeeRecentOrder>();
            }
            return listOfEmployeeRecentOrders;
        }

        private static EmployeeRecentOrder ConvertToEmployeeRecentOrderObject(RecentEmployeeOrderDto employeeOrderDto)
        {
            Item item = new Item { Id = employeeOrderDto.ItemId, Name = employeeOrderDto.ItemName, IsActive = employeeOrderDto.ItemStatus };
            ItemQuantityMapping itemQuantityMapping = new ItemQuantityMapping
            {
                Item = item,
                Quantity = employeeOrderDto.ItemQuantity
            };
            List<ItemQuantityMapping> itemswithqty = new List<ItemQuantityMapping>();
            itemswithqty.Add(itemQuantityMapping);
            Shelf shelf = new Shelf { Id = employeeOrderDto.ShelfId, Name = employeeOrderDto.ShelfName, IsActive = employeeOrderDto.ShelfStatus, Code = employeeOrderDto.ShelfCode };
            Employee employee = new Employee
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
            EmployeeOrderDetails employeeOrderDetails = new EmployeeOrderDetails
            {
                OrderId = employeeOrderDto.OrderId,
                Date = employeeOrderDto.EmployeeOrderDate,
                Shelf = shelf,
                EmployeeItemsQuantityList = itemswithqty,
            };
            return new EmployeeRecentOrder
            {
                Employee = employee,
                EmployeeOrder = employeeOrderDetails
            };
        }

        private RecentEmployeeOrderDto Extract(DbDataReader reader)
        {
            return new RecentEmployeeOrderDto
            {
                EmployeeId = (string)reader["EmployeeId"],
                FirstName = (string)reader["FirstName"],
                LastName = ReturnNullOrValueAccordingly(reader["LastName"]),
                Email = ReturnNullOrValueAccordingly(reader["EmailId"]),
                ContactNumber = ReturnNullOrValueAccordingly(reader["MobileNumber"]),
                TemporaryCardNumber = ReturnNullOrValueAccordingly(reader["TemporaryCardNumber"]),
                AccessCardNumber = ReturnNullOrValueAccordingly(reader["AccessCardNumber"]),
                EmployeeStatus = (bool)reader["IsActive"],
                OrderId = (int)reader["EmployeeOrderId"],
                EmployeeOrderDate = (DateTime)reader["EmployeeOrderDate"],
                ItemQuantity = (int)reader["ItemQuantity"],
                ItemId = (int)reader["ItemId"],
                ItemName = (string)reader["ItemName"],
                ItemStatus = (bool)reader["ItemStatus"],
                ShelfId = (int)reader["ShelfId"],
                ShelfName = (string)reader["ShelfName"],
                ShelfStatus = (bool)reader["ShelfStatus"],
                ShelfCode = (string)reader["ShelfCode"]
            };
        }
        public static string ReturnNullOrValueAccordingly(object field)
        {
            try
            {
                return (string)field;
            }
            catch
            {
                return null;
            }
        }
    }
}