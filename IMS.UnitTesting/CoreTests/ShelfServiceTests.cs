using IMS.Core;
using IMS.DataLayer;
using IMS.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace IMS.UnitTesting.CoreTests
{
    public class ShelfServiceTests
    {
        public Mock<IShelfDbContext> _moqShelfDbContext;
        public ShelfServiceTests()
        {
            _moqShelfDbContext = new Mock<IShelfDbContext>();
        }

        [Fact]
        public void Return_Emptylist_When_There_Are_No_shelves()
        {
            var employeeService = new Core.services.ShelfService(_moqShelfDbContext.Object);
            var response = employeeService.GetShelfList();
            Assert.Equal(Status.Failure, response.Status);
            Assert.Equal(Constants.ErrorCodes.NotFound, response.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.EmptyShelfList, response.Error.ErrorMessage);
        }

        [Fact]
        public void Return_Shelf_When_Shelf_When_Id_Is_Valid()
        {
            var employeeService = new Core.services.ShelfService(_moqShelfDbContext.Object);
            var response = employeeService.GetShelfById("A");
            Assert.Equal(Status.Success, response.Status);
            List<Shelf> shelves = GetShelf();
            Assert.Equal(response.Shelves, shelves);

        }

        [Fact]
        public void Return_All_Shelves()
        {

            throw new NotImplementedException();
        }

        [Fact]
        public void Return_Invalid_Shelf_Id_Message_When_Shelf_Id_Is_Invalid()
        {
            throw new NotImplementedException();
        }

        private List<Shelf> GetShelf()
        {
            List<Shelf> shelves = new List<Shelf>()
            {
                new Shelf()
                {

                   Id = 1,
                   Name = "First Floor",
                   Code ="A",
                   isActive=true

                },

            };
            return shelves;
        }
            
    }
      
}    
