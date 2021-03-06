using IMS.Contracts;
using IMS.Entities.Interfaces;
using IMS.Logging;
using IMS_API.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMS.UnitTesting.ControllerTests
{
    public class ItemControllerTests
    {
        public Mock<IItemService> _moqItemService;
        public Mock<ILogManager> _moqLogManager;
        public ItemControllerTests()
        {
            _moqItemService = new Mock<IItemService>();
            _moqLogManager = new Mock<ILogManager>();
        }

        [Fact]
        public async void GetAllItems_Should_Return_Valid_Response_If_ItemRequestData_Is_Valid()
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
            _moqItemService.Setup(m => m.GetAllItems()).Returns(Task.FromResult(entityItemResponse));
            var itemController = new ItemController(_moqItemService.Object, _moqLogManager.Object);
            IMS.Contracts.ItemResponse actualResult = await itemController.Get();
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }
        [Fact]
        public async void GetItemById_Should_Return_Valid_Response_If_Item_Id_Is_Valid()
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
            _moqItemService.Setup(m => m.GetItemById(1)).Returns(Task.FromResult(entityItemResponse));
            var itemController = new ItemController(_moqItemService.Object, _moqLogManager.Object);
            IMS.Contracts.ItemResponse actualResult = await itemController.Get(1);
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }

        [Fact]
        public async void AddItem_Should_Return_Success_Response_If_Item_Details_Are_Valid()
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
            _moqItemService.Setup(m => m.AddItem(It.Is<Entities.Item>(r => r.Name.Equals("Bottle") && r.MaxLimit.Equals(5)))).Returns(Task.FromResult(entityItemResponse));
            var itemController = new ItemController(_moqItemService.Object, _moqLogManager.Object);
            IMS.Contracts.ItemResponse actualResult = await itemController.Add(new Item { Name = "Bottle", MaxLimit = 5 });
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }

        [Fact]
        public async void UpdateItem_Should_Return_Status_Success_If_Updating_Item_Details_Are_Valid()
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
            _moqItemService.Setup(m => m.UpdateItem(It.Is<Entities.Item>(r => r.Id.Equals(3) && r.Name.Equals("Bag1") && r.MaxLimit.Equals(5)))).Returns(Task.FromResult(entityItemResponse));
            var itemController = new ItemController(_moqItemService.Object, _moqLogManager.Object);
            IMS.Contracts.ItemResponse actualResult = await itemController.Update(new Item { Id = 3, Name = "Bag1", MaxLimit = 5 });
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.NotEmpty(actualResult.Items);
        }
        [Fact]
        public async void DeleteItems_Should_Return_Success_When_Item_Id_Is_Corrected()
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
            _moqItemService.Setup(m => m.Delete(2,false)).Returns(Task.FromResult(entityItemResponse));
            var itemController = new ItemController(_moqItemService.Object, _moqLogManager.Object);
            IMS.Contracts.ItemResponse actualResult = await itemController.Delete(2,false);
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
