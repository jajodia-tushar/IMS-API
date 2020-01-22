using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class EmployeeBulkOrderDbContext:IEmployeeBulkOrderDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeBulkOrderDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<Tuple<int, List<EmployeeBulkOrder>>> GetAllEmployeeBulkOrders(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate)
        {
            DbDataReader reader = null;
            List<EmployeeBulkOrder> employeeBulkOrders = new List<EmployeeBulkOrder>();
            List<EmployeeBulkOrderDto> employeeBulkOrdersDto = new List<EmployeeBulkOrderDto>();
            int limit = pageSize;
            int offset = (pageNumber - 1) * pageSize;
            int totalResuls=0;
            using (var connection = await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "spGetAllBulkOrders";
                command.Parameters.AddWithValue("@lim", limit);
                command.Parameters.AddWithValue("@off", offset);
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);
                command.Parameters.Add("@recordCount", MySqlDbType.Int32, 32);
                command.Parameters["@recordCount"].Direction = ParameterDirection.Output;
                var out_recordCount = command.Parameters["@recordCount"];


                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    employeeBulkOrdersDto.Add(ReadValuesFromDbReader(reader));
                }
                reader.Close();
                if (out_recordCount.Value != DBNull.Value)
                    totalResuls = Convert.ToInt32(out_recordCount.Value);
            }
            employeeBulkOrders = DtoToEntitiesBulkOrders(employeeBulkOrdersDto);
            return Tuple.Create<int,List<EmployeeBulkOrder>>(totalResuls,employeeBulkOrders);
        }
        
        private EmployeeBulkOrderDto ReadValuesFromDbReader(DbDataReader reader)
        {
            return new EmployeeBulkOrderDto()
            {
                EmployeeId = reader["EmployeeId"].ToString() ,
                FirstName = reader["FirstName"].ToString() ,
                LastName = reader["LastName"].ToString() ,
                Email = reader["Email"].ToString() ,
                ContactNumber = reader["ContactNumber"].ToString() ,
                TemporaryCardNumber = reader["TemporaryCardNumber"].ToString() ,
                AccessCardNumber = reader["AccessCardNumber"].ToString() ,
                EmployeeStatus = (bool)reader["EmployeeStatus"] ,
                BulkOrderId = (int)reader["BulkOrderId"] ,
                CreatedOn = (DateTime)reader["CreatedOn"],
                RequirementDate = (DateTime)reader["RequirementDate"],
                RequestStatus = reader["RequestStatus"].ToString(),
                ReasonForRequirement = reader["ReasonForRequirement"].ToString() ,
                ItemId = (int)reader["ItemId"] ,
                ItemName = reader["ItemName"].ToString() ,
                ItemQuantity = (int)reader["ItemQuantity"] ,
                ItemIsActive = (bool)reader["ItemIsActive"] ,
                ItemMaxLimit = (int)reader["ItemMaxLimit"] ,
                ItemImageUrl = reader["ItemImageUrl"].ToString() ,

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
                    ItemQuantityMapping itemQtyMapping = DtoToItemQuantityMapping(bulkOrderDto);
                    mapping[bulkOrderDto.BulkOrderId].EmployeeBulkOrderDetails.EmployeeItemsQuantityList.Add(itemQtyMapping);
                }
                else
                {
                    EmployeeBulkOrder employeeBulkOrder = ToEmployeeBulkOrder(bulkOrderDto);
                    employeeBulkOrder.EmployeeBulkOrderDetails.EmployeeItemsQuantityList.Add(DtoToItemQuantityMapping(bulkOrderDto));
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
                EmployeeItemsQuantityList = new List<ItemQuantityMapping>()
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

        private ItemQuantityMapping DtoToItemQuantityMapping(EmployeeBulkOrderDto bulkOrderDto)
        {
            return new ItemQuantityMapping
            {
                Item = DtoToItem(bulkOrderDto),
                Quantity = bulkOrderDto.ItemQuantity
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
