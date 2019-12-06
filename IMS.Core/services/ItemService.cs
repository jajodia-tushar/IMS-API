using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ItemService : IItemService
    {
        private IItemDbContext _itemDbContext;
        public ItemService(IItemDbContext itemDbContext)
        {
            _itemDbContext = itemDbContext;
        }

        public async Task<ItemResponse> GetAllItems()
        {
            List<Item> itemList;
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                itemList = await _itemDbContext.GetAllItems();
                if (itemList.Count != 0)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = itemList;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ResourceNotFound,
                        ErrorMessage = Constants.ErrorMessages.resourceNotFound
                    };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemResponse;
        }

        public async Task<ItemResponse> GetItemById(int id)
        {
            List<Item> itemList = new List<Item>();
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                Item item = await _itemDbContext.GetItemById(id);
                if (item.Id == id)
                {
                    itemResponse.Status = Status.Success;
                    itemList.Add(item);
                    itemResponse.Items = itemList;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.UnprocessableEntity,
                        ErrorMessage = Constants.ErrorMessages.resourceNotFound
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemResponse;
        }

        public async Task<ItemResponse> AddItem(ItemRequest itemRequest)
        {
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                if (itemRequest.Name == "")
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidItemsDetails
                    };
                    return itemResponse;

                }
                if (await IsItemAlreadyExists(itemRequest.Name))
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.Conflict,
                        ErrorMessage = Constants.ErrorMessages.AlreadyPresent
                    };
                    return itemResponse;
                }
                int latestAddedItemId = await _itemDbContext.AddItem(itemRequest);
                Item item = await _itemDbContext.GetItemById(latestAddedItemId);
                if (item.Name == itemRequest.Name)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = await _itemDbContext.GetAllItems();
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.Conflict,
                        ErrorMessage = Constants.ErrorMessages.Conflict
                    };

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemResponse;
        }

        public async Task<ItemResponse> Delete(int id)
        {
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                if (await isItemAlreadyDeleted(id))
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ResourceNotFound,
                        ErrorMessage = Constants.ErrorMessages.AlreadyDeleted
                    };
                    return itemResponse;
                }
                bool isDeleted = await _itemDbContext.Delete(id);
                if (isDeleted)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = await _itemDbContext.GetAllItems();
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ResourceNotFound,
                        ErrorMessage = Constants.ErrorMessages.resourceNotFound
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemResponse;
        }

        public async Task<ItemResponse> UpdateItem(ItemRequest itemRequest)
        {
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                if (itemRequest.Name == "")
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidItemsDetails
                    };
                    return itemResponse;

                }
                Item item = await _itemDbContext.UpdateItem(itemRequest);
                if (item.Id == itemRequest.Id && item.Name == itemRequest.Name && item.MaxLimit == itemRequest.MaxLimit)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = await _itemDbContext.GetAllItems();
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.Conflict,
                        ErrorMessage = Constants.ErrorMessages.NotUpdated
                    };

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemResponse;
        }

        public async Task<bool> IsItemAlreadyExists(string name)
        {
            List<Item> _items = await _itemDbContext.GetAllItems();
            foreach (var item in _items)
            {
                if (item.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public async Task<bool> isItemAlreadyDeleted(int id)
        {
            List<Item> _items = await _itemDbContext.GetAllItems();
            foreach (var item in _items)
            {
                if (item.Id.Equals(id))
                {
                    if (item.IsActive == false)
                        return true;
                }
            }
            return false;
        }
    }
}
