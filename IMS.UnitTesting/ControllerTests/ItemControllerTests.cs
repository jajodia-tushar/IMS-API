using IMS.Contracts;
using IMS.Entities.Interfaces;
using IMS_API.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IMS.UnitTesting.ControllerTests
{
    public class ItemControllerTests
    {
        public Mock<IItemService> _moqItemService;
        public ItemControllerTests()
        {
            _moqItemService = new Mock<IItemService>();
        }

        [Fact]
        public void GetAllItems_Should_Return_Valid_Response_If_ItemRequestData_Is_Valid()
        {
            var entityItemResponse = new IMS.Entities.ItemResponse()
            {
                Status = IMS.Entities.Status.Success,
                Items = GetEntitiesTypeItemList()
            };
            var expectedResult = new IMS.Contracts.ItemResponse()
            {
                Status = IMS.Contracts.Status.Success,
                Items = GetContractTypeItemsList()
            };
            _moqItemService.Setup(m => m.GetAllItems()).Returns(entityItemResponse);
            var itemController = new ItemController(_moqItemService.Object);
            IMS.Contracts.ItemResponse actualResult = itemController.GetAllItems();
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }
        [Fact]
        public void GetItemById_Should_Return_Valid_Response_If_Item_Id_Is_Valid()
        {
            var entityItemResponse = new IMS.Entities.ItemResponse()
            {
                Status = IMS.Entities.Status.Success,
                Items = GetEntityTypeSingleItemList()
            };
            var expectedResult = new IMS.Contracts.ItemResponse()
            {
                Status = IMS.Contracts.Status.Success,
                Items = GetContractTypeSingleItemList()
            };
            _moqItemService.Setup(m => m.GetItemById(1)).Returns(entityItemResponse);
            var itemController = new ItemController(_moqItemService.Object);
            IMS.Contracts.ItemResponse actualResult = itemController.GetItemById(1);
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }

        [Fact]
        public void AddItem_Should_Return_Success_Response_If_Item_Details_Are_Valid()
        {
            var entityItemResponse = new IMS.Entities.ItemResponse()
            {
                Status = IMS.Entities.Status.Success,
                Items = GetEntitiesTypeItemList()
            };
            var expectedResult = new IMS.Contracts.ItemResponse()
            {
                Status = IMS.Contracts.Status.Success,
                Items = GetContractTypeItemsList()
            };
            _moqItemService.Setup(m => m.AddItem(It.Is<Entities.ItemRequest>(r => r.Name.Equals("Bottle") && r.MaxLimit.Equals(5)))).Returns(entityItemResponse);
            var itemController = new ItemController(_moqItemService.Object);
            IMS.Contracts.ItemResponse actualResult = itemController.AddItem(new ItemRequest { Name = "Bottle", MaxLimit = 5 });
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }

        [Fact]
        public void UpdateItem_Should_Return_Status_Success_If_Updating_Item_Details_Are_Valid()
        {
            var entityItemResponse = new IMS.Entities.ItemResponse()
            {
                Status = IMS.Entities.Status.Success,
                Items = GetEntitiesTypeItemList()
            };
            var expectedResult = new IMS.Contracts.ItemResponse()
            {
                Status = IMS.Contracts.Status.Success,
                Items = GetContractTypeItemsList()
            };
            _moqItemService.Setup(m => m.UpdateItem(It.Is<Entities.ItemRequest>(r => r.Id.Equals(3) && r.Name.Equals("Bag1") && r.MaxLimit.Equals(5)))).Returns(entityItemResponse);
            var itemController = new ItemController(_moqItemService.Object);
            IMS.Contracts.ItemResponse actualResult = itemController.UpdateItem(new ItemRequest { Id = 3, Name = "Bag1", MaxLimit = 5 });
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }
        [Fact]
        public void DeleteItems_Should_Return_Success_When_Item_Id_Is_Corrected()
        {
            var entityItemResponse = new IMS.Entities.ItemResponse()
            {
                Status = IMS.Entities.Status.Success,
                Items = GetEntitiesTypeItemList()
            };
            var expectedResult = new IMS.Contracts.ItemResponse()
            {
                Status = IMS.Contracts.Status.Success,
                Items = GetContractTypeItemsList()
            };
            _moqItemService.Setup(m => m.Delete(2)).Returns(entityItemResponse);
            var itemController = new ItemController(_moqItemService.Object);
            IMS.Contracts.ItemResponse actualResult = itemController.DeleteItems(2);
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }

        public List<IMS.Entities.Item> GetEntitiesTypeItemList()
        {
            List<IMS.Entities.Item> _entityTypeItemsList = new List<IMS.Entities.Item>()
            {
                new Entities.Item()
                {
                    Id = 1,
                    Name = "Pen",
                    MaxLimit = 5,
                    IsActive = true
                },
                new Entities.Item()
                {
                    Id = 2,
                    Name = "Bottle",
                    MaxLimit = 5,
                    IsActive = false
                },
                new Entities.Item()
                {
                    Id = 3,
                    Name = "Bag1",
                    MaxLimit = 5,
                    IsActive = true
                },
                new Entities.Item()
                {
                    Id = 4,
                    Name = "Bag",
                    MaxLimit = 5,
                    IsActive = true
                }
            };
            return _entityTypeItemsList;
        }
        public List<IMS.Contracts.Item> GetContractTypeItemsList()
        {
            List<IMS.Contracts.Item> _contractsTypeItemList = new List<IMS.Contracts.Item>()
            {
                new Contracts.Item()
                {
                    Id = 1,
                    Name = "Pen",
                    MaxLimit = 5,
                    IsActive = true
                },
                new Contracts.Item()
                {
                    Id = 2,
                    Name = "Pencil",
                    MaxLimit = 4,
                    IsActive = true
                },
                new Contracts.Item()
                {
                    Id = 3,
                    Name = "Bottle",
                    MaxLimit = 5,
                    IsActive = true
                }
            };
            return _contractsTypeItemList;
        }
        public List<IMS.Entities.Item> GetEntityTypeSingleItemList()
        {
            List<IMS.Entities.Item> _entityTypeSingleItemList = new List<IMS.Entities.Item>()
            {
                new Entities.Item()
                {
                    Id = 1,
                    Name = "Pen",
                    MaxLimit = 5,
                    IsActive = true
                },
            };
            return _entityTypeSingleItemList;
        }
        public List<IMS.Contracts.Item> GetContractTypeSingleItemList()
        {
            List<IMS.Contracts.Item> _contractTypeSingleItemList = new List<IMS.Contracts.Item>()
            {
                new Contracts.Item()
                {
                    Id = 1,
                    Name = "Pen",
                    MaxLimit = 5,
                    IsActive = true
                },
            };
            return _contractTypeSingleItemList;
        }
    }
}
