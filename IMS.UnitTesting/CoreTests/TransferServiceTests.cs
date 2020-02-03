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
    public class TransferServiceTests
    {
        public Mock<ITransferDbContext> _moqTransferDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;

        public TransferServiceTests()
        {
            _moqTransferDbContext = new Mock<ITransferDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _moqTokenProvider.Setup(t => t.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "bearer " + Tokens.SuperAdmin;
            _moqHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async void TransferToShelves_Should_Return_Error_When_Token_Is_InValid()
        {
            _moqTransferDbContext.Setup(m => m.TransferToShelves(It.IsAny<TransferToShelvesRequest>())).Returns(Task.FromResult(true));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(false));
            var transferServiceObject = new TransferService(_moqTransferDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await transferServiceObject.TransferToShelves(new TransferToShelvesRequest());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void TransferToShelves_Should_Return_Error_When_Request_Is_InValid()
        {
            _moqTransferDbContext.Setup(m => m.TransferToShelves(It.IsAny<TransferToShelvesRequest>())).Returns(Task.FromResult(true));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var transferServiceObject = new TransferService(_moqTransferDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await transferServiceObject.TransferToShelves(new TransferToShelvesRequest());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void TransferToShelves_Should_Return_Error_When_Transfer_Is_Failed()
        {
            _moqTransferDbContext.Setup(m => m.TransferToShelves(It.IsAny<TransferToShelvesRequest>())).Returns(Task.FromResult(false));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var transferServiceObject = new TransferService(_moqTransferDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await transferServiceObject.TransferToShelves(new TransferToShelvesRequest());
            Assert.Equal(Status.Failure, resultant.Status);
        }
        [Fact]
        public async void TransferToShelves_Should_Return_Success_When_Request_Is_Valid()
        {
            _moqTransferDbContext.Setup(m => m.TransferToShelves(It.IsAny<TransferToShelvesRequest>())).Returns(Task.FromResult(true));
            _moqTokenProvider.Setup(m => m.IsValidToken(It.IsAny<string>())).Returns(Task.FromResult(true));
            var transferServiceObject = new TransferService(_moqTransferDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await transferServiceObject.TransferToShelves(GetValidTransferRequest());
            Assert.Equal(Status.Success, resultant.Status);
        }

        public TransferToShelvesRequest GetValidTransferRequest()
        {
            return new TransferToShelvesRequest() {
                ShelvesItemsQuantityList = new List<TransferToShelfRequest>()
                {
                    new TransferToShelfRequest
                    {
                        Shelf = new Shelf(){
                            Id='1',
                            Name="FirstFloor",
                            Code="A",
                            IsActive=true
                        },
                        ItemQuantityMapping = new List<ItemQuantityMapping>()
                        {
                            new ItemQuantityMapping()
                            {
                                Item=new Item()
                                {
                                    Id=1
                                },
                                Quantity=2
                            }
                        }
                    }
                }
            };
        }
    }
}