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

        public async Task<List<EmployeeRecentOrder>> GetRecentEmployeeOrders(int pageNumber, int pageSize)
        {
            MySqlDataReader reader1 = null;
            List<RecentEmployeeOrderDto> recentEmployeeOrderDtos = new List<RecentEmployeeOrderDto>();
            List<EmployeeRecentOrder> listOfEmployeeRecentOrders = new List<EmployeeRecentOrder>();
            int limit = pageSize;

            int offset = (pageNumber - 1) * pageSize;

            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllEmployeeRecentOrders";
                    command.Parameters.AddWithValue("@lim", limit);
                    command.Parameters.AddWithValue("@off", offset);
                    reader1 = command.ExecuteReader();
                    while (reader1.Read())
                    {
                        RecentEmployeeOrderDto employeeOrder = Extract(reader1);
                        recentEmployeeOrderDtos.Add(employeeOrder);
                    }
                    listOfEmployeeRecentOrders = GetListOfEmployeeRecentOrder(recentEmployeeOrderDtos);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return listOfEmployeeRecentOrders;
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
    }
}
