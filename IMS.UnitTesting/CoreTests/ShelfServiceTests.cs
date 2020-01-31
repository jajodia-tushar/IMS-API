using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.TokenManagement;
using Xunit;
namespace IMS.UnitTesting.CoreTests
{
    public class ShelfServiceTests
    {
        public Mock<IShelfDbContext> _moqShelfDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;
        public ShelfServiceTests()
        {
            _moqShelfDbContext = new Mock<IShelfDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();
            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public async void Return_Emptylist_When_There_Are_No_shelves()
        {
            List<Shelf> shelves = new List<Shelf>();
            _moqShelfDbContext.Setup(p => p.GetAllShelves()).Returns(Task.FromResult(shelves));
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.GetShelfList();
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.EmptyShelfList, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void Return_Shelf_When_Shelf_Id_Is_Valid()
        {
            _moqShelfDbContext.Setup(p => p.GetShelfByShelfCode("A")).Returns(Task.FromResult(GetShelfByCode()));
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.GetShelfByShelfCode("A");
            Assert.Equal(Status.Success, resultant.Status);
        }

        [Fact]
        public async void Return_All_Shelves()
        {
            _moqShelfDbContext.Setup(p => p.GetAllShelves()).Returns(GetShelvesList());
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.GetShelfList();
            Assert.Equal(Status.Success, resultant.Status);
        }

        [Fact]
        public async void Return_Invalid_Shelf_Id_Message_When_Shelf_Id_Is_Invalid()
        {
            Shelf shelf = new Shelf();
            _moqShelfDbContext.Setup(p => p.GetShelfByShelfCode("D")).Returns(Task.FromResult(shelf));
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.GetShelfByShelfCode("D");
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidShelfCode, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void Return_Success_When_New_Shelf_Added()
        {
            _moqShelfDbContext.Setup(p => p.AddShelf(It.Is<Shelf>(r => r.Name.Equals("Ninth Floor") && r.Code.Equals("D") && r.IsActive.Equals(true)))).Returns(Task.FromResult(GetNewShelf()));
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.AddShelf(new Shelf { Name = "Ninth Floor", Code = "D" ,IsActive = true});
            Assert.Equal(Status.Success, resultant.Status);
            Assert.Null(resultant.Error);
        }

        [Fact]
        public async void Return_Failure_When_Shelf_Is_Already_Added()
        {
            _moqShelfDbContext.Setup(p => p.AddShelf(It.Is<Shelf>(r => r.Name.Equals("First Floor") && r.Code.Equals("A") && r.IsActive.Equals(true)))).Returns(Task.FromResult(GetNewShelf()));
            _moqShelfDbContext.Setup(p => p.IsShelfPresentByCode("A")).Returns(Task.FromResult(true));
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.AddShelf(new Shelf { Name = "First Floor", Code = "A", IsActive = true });
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest,resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.ShelfIsAlreadyPresent, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void Return_Success_When_Shelf_Is_Deleted()
        {
            _moqShelfDbContext.Setup(p => p.IsShelfPresentByCode("A")).Returns(Task.FromResult(true));
            _moqShelfDbContext.Setup(p => p.GetShelfStatusByCode("A")).Returns(Task.FromResult(true));
            _moqShelfDbContext.Setup(p => p.DeleteShelfByCode("A")).Returns(GetShelvesList());
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.Delete("A");
            Assert.Equal(Status.Success, resultant.Status);
        }

        [Fact]
        public async void Return_Failure_When_Shelf_Is_Not_Deleted()
        {
            _moqShelfDbContext.Setup(p => p.IsShelfPresentByCode("A")).Returns(Task.FromResult(false));
            _moqShelfDbContext.Setup(p => p.GetShelfStatusByCode("A")).Returns(Task.FromResult(false));
            _moqShelfDbContext.Setup(p => p.DeleteShelfByCode("A")).Returns(GetShelvesList());
            var shelfServiceObject = new ShelfService(_moqShelfDbContext.Object, _moqLogManager.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object);
            var resultant = await shelfServiceObject.Delete("A");
            Assert.Equal(Status.Failure, resultant.Status);
        }
    
       
        private List<Shelf> GetNewShelf()
        {
            return new List<Shelf>()
            {
                new Shelf()
                {
                    Id  = 4,
                    IsActive = true,
                    Name = "Ninth Floor",
                    Code = "D"
                }
            };
        }

        Task<List<Shelf>> GetShelvesList()
        {
            List<Shelf> shelves = new List<Shelf>()
            {
                new Shelf()
                {
                    Id = 1,
                    Code = "A",
                    IsActive = true,
                    Name = "First Floor"
                },
                new Shelf()
                {
                    Id = 2,
                    Code = "3",
                    IsActive = true,
                    Name = "Sixth Floor"
                },
                new Shelf()
                {
                    Id = 3,
                    Code = "C",
                    IsActive = true,
                    Name = "Eighth"
                }
            };
            return Task.FromResult(shelves);
        }

        Shelf GetShelfByCode()
        {
            return new Shelf()
            { 
                    Id = 1,
                    Code = "A",
                    IsActive = true,
                    Name = "First Floor"
            };
        }
    }
      
}    
