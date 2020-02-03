using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class VendorServiceTests
    {
        public Mock<IVendorDbContext> _moqVendorDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;

        public VendorServiceTests()
        {
            _moqVendorDbContext = new Mock<IVendorDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async void GetAllVendors_Should_Return_Valid_Response_And_Return_All_Vendors()
        {
            _moqVendorDbContext.Setup(m => m.GetVendors("Vendor1",10,0)).Returns(GetAllVendorsResposnse());
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.GetVendors("Vendor1",1,10);
            Assert.Equal(Status.Success, resultant.Status);
        }

        [Fact]
        public async void GetAllVendors_Should_Return_Error_When_Token_Is_Invalid()
        {
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _moqVendorDbContext.Setup(m => m.GetVendors("Vendor1", 10, 0)).Returns(GetAllVendorsResposnse());
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.GetVendors("Vendor1", 1, 10);
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void GetVendorById_Should_Return_Error_When_Id_Is_Invalid()
        {
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.GetVendorById(0);
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void GetVendorById_Should_Return_Vendor_When_Id_Is_Valid()
        {
            _moqVendorDbContext.Setup(m => m.GetVendorById(1)).Returns(GetAllVendorById1);
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.GetVendorById(1);
            Assert.Equal(Status.Success, resultant.Status);
        }
        [Fact]
        public async void UpdateVendor_Should_Return_UpdatedVendor_When_Request_Is_Valid()
        {
            _moqVendorDbContext.Setup(m => m.UpdateVendor(It.IsAny<Vendor>())).Returns(GetAllVendorById1());
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.UpdateVendor(GetValidVendor());
            Assert.Equal(Status.Success, resultant.Status);
        }
        [Fact]
        public async void UpdateVendor_Should_Return_Error_When_Request_Is_InValid()
        {
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.UpdateVendor(new Vendor());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void AddVendor_Should_Return_Success_When_Request_Is_Valid()
        {
            _moqVendorDbContext.Setup(m => m.UpdateVendor(It.IsAny<Vendor>())).Returns(GetAllVendorById1());
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.AddVendor(GetValidVendor());
            Assert.Null(resultant.Error);
        }
        [Fact]
        public async void AddVendor_Should_Return_Error_When_Request_Is_InValid()
        {
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.AddVendor(new Vendor() { Id = 0 });
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void IsVendorUnique_Should_Return_Success_When_Vendor_Is_Unique()
        {
            _moqVendorDbContext.Setup(m => m.IsVendorPresent(GetValidVendor())).Returns(Task.FromResult(true));
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.AddVendor(GetValidVendor());
            Assert.Null(resultant.Error);
        }
        [Fact]
        public async void IsVendorUnique_Should_Return_Error_When_Vendor_Is_Present_In_Database()
        {
            _moqVendorDbContext.Setup(m => m.IsVendorPresent(It.IsAny<Vendor>())).Returns(Task.FromResult(false));
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.AddVendor(new Vendor() { Id = 0 });
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void DeleteVendor_Should_Return_Success_When_Vendor_Is_Deleted()
        {
            _moqVendorDbContext.Setup(m => m.DeleteVendor(It.IsAny<int>(),It.IsAny<bool>())).Returns(Task.FromResult(true));
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.DeleteVendor(1,true);
            Assert.Null(resultant.Error);
        }
        [Fact]
        public async void DeleteVendor_Should_Return_Error_When_Vendor_Is_Not_Deleted()
        {
            _moqVendorDbContext.Setup(m => m.DeleteVendor(It.IsAny<int>(),It.IsAny<bool>())).Returns(Task.FromResult(false));
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.DeleteVendor(1, true);
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void DeleteVendor_Should_Return_Error_When_Vendor_Id_Is_InValid()
        {
            var vendorServiceObject = new VendorService(_moqVendorDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await vendorServiceObject.DeleteVendor(0, true);
            Assert.Equal(Status.Failure, resultant.Status);
        }
        private Vendor GetValidVendor()
        {
            return new Vendor()
            {
                Id = 1,
                Name = "Vendor1",
                Address="Pune",
                PAN="PAN",
                GST="GST",
                CompanyIdentificationNumber="CIN",
                ContactNumber="1234",
                Title="Pencil Guy"
            };
        }
        private async Task<Vendor> GetAllVendorById1()
        {
            return new Vendor()
            {
                Id = 1,
                Name = "Vendor1"
            };
        }

        private async Task<VendorsResponse> GetAllVendorsResposnse()
        {
            VendorsResponse vendorsResponse = new VendorsResponse()
            {
                Status = Status.Success,
                Error = null,
                Vendors = new List<Vendor>()
                {
                new Vendor()
                {
                    Id=1,
                    Name="Vendor1"
                },
                new Vendor()
                {
                    Id=2,
                    Name="Vendor12"
                },
                new Vendor()
                {
                    Id=3,
                    Name="Vendor11"
                }
                },
                PagingInfo = new PagingInfo()
                {
                    PageNumber = 1,
                    PageSize = 10,
                    TotalResults = 100
                }
            }; 
            return vendorsResponse;
        }
    }
}