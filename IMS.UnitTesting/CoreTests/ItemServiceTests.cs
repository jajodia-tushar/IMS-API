using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class ItemServiceTests
    {
        public Mock<IItemDbContext> _moqItemDbContext;
        public ItemServiceTests()
        {
            _moqItemDbContext = new Mock<IItemDbContext>();
        }

        [Fact]
        public void GetAllItems_Should_Return_Valid_Response_And_Return_All_Items_When_Items_Are_Present_In_Database()
        {
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetAllItemsList());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.GetAllItems();
            Assert.Equal(Status.Success, resultant.Status);
            //Assert.Equal(3, resultant.Items.Count);
        }
        [Fact]
        public void GetAllItems_Should_Return_Error_And_Status_Failure_When_Items_Are_Not_Present_In_Database()
        {
            List<Item> _items = new List<Item>();
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(_items);
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.GetAllItems();
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.ResourceNotFound, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.resourceNotFound, resultant.Error.ErrorMessage);
        }

        [Fact]
        public void GetItemById_Should_Return_Status_Success_And_Valid_Item_When_Item_Id_Is_Present_In_Database()
        {
            _moqItemDbContext.Setup(m => m.GetItemById(1)).Returns(GetOneItem());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.GetItemById(1);
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotEmpty(resultant.Items);
        }

        [Fact]
        public void GetItemById_Should_Return_Error_And_Status_Failure_When_Item_Id_Is_Not_Present()
        {
            List<Item> _items = new List<Item>();
            _moqItemDbContext.Setup(m => m.GetItemById(1)).Returns(_items);
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.GetItemById(1);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.UnprocessableEntity, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.resourceNotFound, resultant.Error.ErrorMessage);
        }

        [Fact]
        public void AddItem_Should_Return_Error_And_Status_Failure_When_Item_Name_Is_Null()
        {
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.AddItem(new ItemRequest() { Name = "", MaxLimit = 4 });
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidItemsDetails, resultant.Error.ErrorMessage);
        }

        [Fact]
        public void AddItem_Should_Return_Success_When_Item_Details_Is_Valid()
        {
            _moqItemDbContext.Setup(m => m.AddItem(It.Is<ItemRequest>(r => r.Name.Equals("Bag") && r.MaxLimit.Equals(5)))).Returns(GetAllItemsList());
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.AddItem(new ItemRequest() { Name = "Bag", MaxLimit = 5 });
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public void Delete_Method_Should_Return_Status_Success_When_Item_Is_Deleted_Correct()
        {
            _moqItemDbContext.Setup(m => m.Delete(2)).Returns(GetAllItemsList());
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.Delete(2);
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public void Delete_Method_Should_Return_Status_Failure_When_Item_Is_Not_Deleted()
        {
            _moqItemDbContext.Setup(m => m.Delete(3)).Returns(GetAllItemsList());
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.Delete(3);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Null(resultant.Items);
        }

        [Fact]
        public void Update_Item_Should_Return_Success_After_Updating_Valid_Details()
        {
            _moqItemDbContext.Setup(m => m.UpdateItem(It.Is<int>(r=>r.Equals(3)),It.Is<ItemRequest>(r => r.Name.Equals("Bag1") && r.MaxLimit.Equals(5)))).Returns(GetAllItemsList());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.UpdateItem(3,new ItemRequest() { Name = "Bag1", MaxLimit = 5 });
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public void Update_Item_Should_Return_Status_Failure_When_Updating_Item_Details_Is_Not_Valid()
        {
            var itemServiceObject = new ItemService(_moqItemDbContext.Object);
            var resultant = itemServiceObject.UpdateItem(3, new ItemRequest() { Name = "", MaxLimit = 5 });
            Assert.Equal(Status.Failure, resultant.Status);
        }

        List<Item> GetAllItemsList()
        {
            List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   isActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Bootle",
                   MaxLimit = 5,
                   isActive = false
               },
               new Item()
               {
                   Id =3,
                   Name = "Bag1",
                   MaxLimit = 5,
                   isActive = true
               },
               new Item()
               {
                   Id =4,
                   Name = "Bag",
                   MaxLimit = 5,
                   isActive = true
               }
            };
            return _items;
        }

        List<Item> GetOneItem()
        {
            List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   isActive = true
               }
            };
            return _items;
        }
        List<Item> GetItems()
        {
            List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   isActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Pen",
                   MaxLimit = 4,
                   isActive = true
               }
            };
            return _items;
        }

    }

}