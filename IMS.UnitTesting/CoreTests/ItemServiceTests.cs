﻿using IMS.Core;
using IMS.Core.services;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.CoreTests
{
    public class ItemServiceTests
    {
        public Mock<IItemDbContext> _moqItemDbContext;
        public Mock<ITokenProvider> _moqTokenProvider;
        public Mock<ILogManager> _moqLogManager;
        public Mock<IHttpContextAccessor> _moqHttpContextAccessor;

        public ItemServiceTests()
        {
            _moqItemDbContext = new Mock<IItemDbContext>();
            _moqTokenProvider = new Mock<ITokenProvider>();
            _moqLogManager = new Mock<ILogManager>();

            _moqHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public async void GetAllItems_Should_Return_Valid_Response_And_Return_All_Items_When_Items_Are_Present_In_Database()
        {
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetAllItemsList());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.GetAllItems();
            Assert.Equal(Status.Success, resultant.Status);
            //Assert.Equal(3, resultant.Items.Count);
        }
        [Fact]
        public async void GetAllItems_Should_Return_Error_And_Status_Failure_When_Items_Are_Not_Present_In_Database()
        {
            List<Item> _items = new List<Item>();
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(Task.FromResult(_items));
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.GetAllItems();
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.ResourceNotFound, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.resourceNotFound, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void GetItemById_Should_Return_Status_Success_And_Valid_Item_When_Item_Id_Is_Present_In_Database()
        {
            _moqItemDbContext.Setup(m => m.GetItemById(3)).Returns(Task.FromResult(GetOneItem()));
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.GetItemById(3);
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotEmpty(resultant.Items);
        }

        [Fact]
        public async void GetItemById_Should_Return_Error_And_Status_Failure_When_Item_Id_Is_Not_Present()
        {
            Item item = new Item();
            _moqItemDbContext.Setup(m => m.GetItemById(3)).Returns(Task.FromResult(item));
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.GetItemById(3);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.UnprocessableEntity, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.resourceNotFound, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void AddItem_Should_Return_Error_And_Status_Failure_When_Item_Name_Is_Null()
        {
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.AddItem(new ItemRequest() { Name = "", MaxLimit = 4 });
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Equal(Constants.ErrorCodes.BadRequest, resultant.Error.ErrorCode);
            Assert.Equal(Constants.ErrorMessages.InvalidItemsDetails, resultant.Error.ErrorMessage);
        }

        [Fact]
        public async void AddItem_Should_Return_Success_When_Item_Details_Is_Valid()
        {
            _moqItemDbContext.Setup(m => m.AddItem(It.Is<ItemRequest>(r => r.Name.Equals("Bag1") && r.MaxLimit.Equals(5)))).Returns(Task.FromResult(3));
            _moqItemDbContext.Setup(m => m.GetItemById(3)).Returns(Task.FromResult(GetOneItem()));
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.AddItem(new ItemRequest() { Name = "Bag1", MaxLimit = 5 });
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public async void Delete_Method_Should_Return_Status_Success_When_Item_Is_Deleted_Correct()
        {
            _moqItemDbContext.Setup(m => m.Delete(2)).Returns(Task.FromResult(true));
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.Delete(2);
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public async void Delete_Method_Should_Return_Status_Failure_When_Item_Is_Not_Deleted()
        {
            _moqItemDbContext.Setup(m => m.Delete(3)).Returns(Task.FromResult(false));
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetItems());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.Delete(3);
            Assert.Equal(Status.Failure, resultant.Status);
            Assert.Null(resultant.Items);
        }

        [Fact]
        public async void Update_Item_Should_Return_Success_After_Updating_Valid_Details()
        {
            _moqItemDbContext.Setup(m => m.UpdateItem(It.Is<ItemRequest>(r => r.Id.Equals(3) && r.Name.Equals("Bag1") && r.MaxLimit.Equals(5)))).Returns(Task.FromResult(GetOneItem()));
            _moqItemDbContext.Setup(m => m.GetAllItems()).Returns(GetAllItemsList());
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.UpdateItem(new ItemRequest() { Id = 3, Name = "Bag1", MaxLimit = 5 });
            Assert.Equal(Status.Success, resultant.Status);
            Assert.NotNull(resultant.Items);
        }

        [Fact]
        public async void Update_Item_Should_Return_Status_Failure_When_Updating_Item_Details_Is_Not_Valid()
        {
            var itemServiceObject = new ItemService(_moqItemDbContext.Object, _moqTokenProvider.Object, _moqHttpContextAccessor.Object, _moqLogManager.Object);
            var resultant = await itemServiceObject.UpdateItem(new ItemRequest() { Id = 3, Name = "", MaxLimit = 5 });
            Assert.Equal(Status.Failure, resultant.Status);
        }

        Task<List<Item>> GetAllItemsList()
        {
            List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   IsActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Bootle",
                   MaxLimit = 5,
                   IsActive = false
               },
               new Item()
               {
                   Id =3,
                   Name = "Bag1",
                   MaxLimit = 5,
                   IsActive = true
               },
               new Item()
               {
                   Id =4,
                   Name = "Bag",
                   MaxLimit = 5,
                   IsActive = true
               }
            };
            return Task.FromResult(_items);
        }

        Item GetOneItem()
        {
            return new Item()
            {
                Id = 3,
                Name = "Bag1",
                MaxLimit = 5,
                IsActive = true
            };
        }
        
        Task<List<Item>> GetItems()
        {
            List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   IsActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Pen",
                   MaxLimit = 4,
                   IsActive = true
               }
            };
            return Task.FromResult(_items);
        }

    }

}