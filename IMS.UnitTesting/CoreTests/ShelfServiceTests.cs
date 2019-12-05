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
            throw new NotImplementedException();
        }

        [Fact]
        public void Return_Shelf_When_Shelf_Id_Is_Valid()
        {

            throw new NotImplementedException();

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

      
            
    }
      
}    
