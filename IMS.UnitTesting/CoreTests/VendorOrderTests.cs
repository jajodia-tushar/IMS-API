using IMS.Contracts;
using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class VendorOrderTests
    {
        private Mock<IVendorService> _moqVendorServices;
        private Mock<IVendorOrderDbContext> _moqVendorOrderDbContext;
        private Mock<IHttpContextAccessor> _moqHttpContextAccessor;
        private Mock<ITokenProvider> _moqTokenProvider;
        public VendorOrderTests()
        {
            _moqVendorServices = new Mock<IVendorService>();
            _moqVendorOrderDbContext = new Mock<IVendorOrderDbContext>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _moqTokenProvider = new Mock<ITokenProvider>();
        }
        [Fact]
        public async void Return_Records_Not_Found_Error_For_Empty_List_Of_Orders()
        {
            var dateTime = new DateTime();
            _moqVendorOrderDbContext.Setup(m => m.GetVendorOrders(false,1,10,dateTime,dateTime)).Returns(GetVendorOrders(1));
        }

        private async Task<VendorOrdersDto> GetVendorOrders(int numberOfVendorOrders)
        {
            return new VendorOrdersDto()
            {
                VendorOrders = GetVendorOrdersList(numberOfVendorOrders),
                TotalRecords = numberOfVendorOrders
            };
        }

        private List<Entities.VendorOrder> GetVendorOrdersList(int size)
        {
            var vendorOrders = new List<Entities.VendorOrder>();
            for(int i = 0; i < size; i++)
            {
                var vendorOrder = new VendorOrder()
                {
                    Vendor = GetVendor(),
                    VendorOrderDetails = GetVendorOrderDetails()
                };
            }
            return vendorOrders;
        }

        private VendorOrderDetails GetVendorOrderDetails()
        {
            return new VendorOrderDetails()
            {
                ChallanImageUrl = "sdf",
                ChallanNumber = "asdf",
                Date = new DateTime(),
                InvoiceImageUrl = "asdf",
                InvoiceNumber = "asddf",
                RecievedBy = "asdfd",
                SubmittedTo = "asdf",
                IsApproved = false,
                OrderId = 112,
                TaxableAmount = 122,
                OrderItemDetails = GetOrderItemDetails()
            };
        }

        private List<ItemQuantityPriceMapping> GetOrderItemDetails()
        {
            var itemQuantitiyPriceMappings = new List<ItemQuantityPriceMapping>();
            itemQuantitiyPriceMappings.Add(GetItemQuantityPriceMapping());
            return itemQuantitiyPriceMappings;
        }

        private ItemQuantityPriceMapping GetItemQuantityPriceMapping()
        {
            return new ItemQuantityPriceMapping()
            {
                Item = GetItem(),
                Quantity = 3,
                TotalPrice = 65,
            };
        }

        private Item GetItem()
        {
            return new Item()
            {
                Id = 1,
                ImageUrl = "asdf",
                IsActive = true,
                MaxLimit = 4,
                Name = "Pen",
                Rate = 23,
                ShelvesAmberLimit = 23,
                ShelvesRedLimit = 12,
                WarehouseAmberLimit = 11,
                WarehouseRedLimit = 9,
            };
        }

        private Vendor GetVendor()
        {
            return new Vendor()
            {
                Title = "asdf",
                GST = "asdf",
                Address = "asdf",
                CompanyIdentificationNumber = "fda",
                ContactNumber = "fsda",
                Id = 1,
                Name = "dasf",
                PAN = "dss"
            };
        }
    }
}
