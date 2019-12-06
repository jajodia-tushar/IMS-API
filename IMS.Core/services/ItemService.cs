using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMS.Core.services
{
    public class ItemService : IItemService
    {
        private IItemDbContext _itemDbContext;
        public ItemService(IItemDbContext itemDbContext)
        {
            _itemDbContext = itemDbContext;
        }

        public ItemResponse GetAllItems()
        {
            List<Item> _itemList;
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                _itemList = _itemDbContext.GetAllItems();
                if (_itemList.Count != 0)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemList;
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

        public ItemResponse GetItemById(int id)
        {
            List<Item> _itemList = new List<Item>();
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                Item item = _itemDbContext.GetItemById(id);
                if (item.Id == id)
                {
                    itemResponse.Status = Status.Success;
                    _itemList.Add(item);
                    itemResponse.Items = _itemList;
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

        public ItemResponse AddItem(ItemRequest itemRequest)
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
                if (IsItemAlreadyExists(itemRequest.Name))
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.Conflict,
                        ErrorMessage = Constants.ErrorMessages.AlreadyPresent
                    };
                    return itemResponse;
                }
                int latestAddedItemId = _itemDbContext.AddItem(itemRequest);
                Item item = _itemDbContext.GetItemById(latestAddedItemId);
                if (item.Name == itemRequest.Name)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemDbContext.GetAllItems();
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

        public ItemResponse Delete(int id)
        {
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                if (isItemAlreadyDeleted(id))
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ResourceNotFound,
                        ErrorMessage = Constants.ErrorMessages.AlreadyDeleted
                    };
                    return itemResponse;
                }
                bool isDeleted = _itemDbContext.Delete(id);
                if (isDeleted)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemDbContext.GetAllItems();
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

        public ItemResponse UpdateItem(ItemRequest itemRequest)
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
                Item item = _itemDbContext.UpdateItem(itemRequest);
                if (item.Id == itemRequest.Id && item.Name == itemRequest.Name && item.MaxLimit == itemRequest.MaxLimit)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemDbContext.GetAllItems();
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

        public bool IsItemAlreadyExists(string name)
        {
            List<Item> _items = _itemDbContext.GetAllItems();
            foreach (var item in _items)
            {
                if (item.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public bool isItemAlreadyDeleted(int id)
        {
            List<Item> _items = _itemDbContext.GetAllItems();
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
