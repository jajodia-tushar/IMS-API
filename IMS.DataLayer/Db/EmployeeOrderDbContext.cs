using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
            MySqlDataReader reader = null;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
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
                    reader = command.ExecuteReader();
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
            MySqlDataReader reader1 = null;
            List<EmployeeOrderDetails> employeeOrders = new List<EmployeeOrderDetails>();
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetEmployeeOrdersByEmployeeId";
                    command.Parameters.AddWithValue("@Id", id);
                    reader1 = command.ExecuteReader();
                    while (reader1.Read())
                    {
                        employeeOrders.Add(new EmployeeOrderDetails()
                        {
                            OrderId = (int)reader1["Id"],
                            Date = (DateTime)reader1["Date"],
                            Shelf = new Shelf()
                            {
                                Id = (Int32)reader1["ShelfId"],
                                Name = (string)reader1["ShelfName"],
                                Code = (string)reader1["ShelfCode"],
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
                        reader1 = command2.ExecuteReader();
                        while (reader1.Read())
                        {
                            currentOrder.EmployeeItemsQuantityList.Add(new ItemQuantityMapping
                            {
                                Item = new Item()
                                {
                                    Name = (string)reader1["Name"],
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
            MySqlDataReader reader = null;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.pageNumber = pageNumber;
            pagingInfo.pageSize = pageSize;
            EmployeeRecentOrderResponse employeeRecentOrderResponse = new EmployeeRecentOrderResponse();
            List<RecentEmployeeOrderDto> recentEmployeeOrderDtos = new List<RecentEmployeeOrderDto>();
            List<EmployeeRecentOrder> listOfEmployeeRecentOrders = new List<EmployeeRecentOrder>();
            int limit =pageSize;
            int offset = (pageNumber - 1) *pageSize;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = " spGetEmployeeRecentOrdersTest";
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    command.Parameters.Add("@orderCount", MySqlDbType.Int32, 32);
                    command.Parameters["@orderCount"].Direction = ParameterDirection.Output;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        RecentEmployeeOrderDto employeeOrder = Extract(reader);
                        recentEmployeeOrderDtos.Add(employeeOrder);
                    }
                    reader.Close();
                   
                    command.ExecuteNonQuery();
                    pagingInfo.totalResults = (int)command.Parameters["@orderCount"].Value;
                    listOfEmployeeRecentOrders = GetListOfEmployeeRecentOrder(recentEmployeeOrderDtos);
                    employeeRecentOrderResponse.EmployeeRecentOrders = listOfEmployeeRecentOrders;
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

        private RecentEmployeeOrderDto Extract(MySqlDataReader reader)
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
    }
}