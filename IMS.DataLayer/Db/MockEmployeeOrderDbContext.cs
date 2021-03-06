using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class MockEmployeeOrderDbContext : IEmployeeOrderDbContext
    {
        private static List<EmployeeOrderDetails> _employeeOrders;
        public MockEmployeeOrderDbContext()
        {
            _employeeOrders = new List<EmployeeOrderDetails>()
            {
                new EmployeeOrderDetails()
                {
                    Date= new DateTime(1998, 04, 30, 0, 0, 0),
                    Shelf=new Shelf()
                    {
                        Id =1
                    },
                    OrderId=1,
                    EmployeeItemsQuantityList=new List<ItemQuantityMapping>()
                    {
                        new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id=1
                            },
                            Quantity = 2
                        },
                        new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id=3
                            },
                            Quantity = 5
                        }
                    }
                },
                new EmployeeOrderDetails()
                {
                    Date= new DateTime(1997, 05, 31, 0, 0, 0),
                    Shelf=new Shelf()
                    {
                        Id =2
                    },
                    OrderId=2,
                    EmployeeItemsQuantityList=new List<ItemQuantityMapping>()
                    {
                        new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id=3
                            },
                            Quantity = 2
                        },
                        new ItemQuantityMapping()
                        {
                            Item = new Item()
                            {
                                Id=4
                            },
                            Quantity = 5
                        }
                    }
                }
            };
        }

        public Task<EmployeeOrder> AddEmployeeOrder(EmployeeOrder employeeOrder)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId, int limit, int offset, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeOrderResponse> GetRecentEmployeeOrders(int pageSize, int pageNumber)
        {
            throw new NotImplementedException();
        }
    }
}
