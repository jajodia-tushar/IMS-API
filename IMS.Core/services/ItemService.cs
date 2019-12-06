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
            List<Item> _itemList;
            ItemResponse itemResponse = new ItemResponse();
            try
            {
                _itemList = _itemDbContext.GetItemById(id);
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
            List<Item> _itemList;
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
                _itemList = _itemDbContext.AddItem(itemRequest);
                if (_itemList[_itemList.Count - 1].Name == itemRequest.Name)
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemList;
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
            List<Item> _itemList;
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
                _itemList = _itemDbContext.Delete(id);
                if ((_itemList.FindAll(i => i.Id == id && i.isActive == false)).Any())
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

        public ItemResponse UpdateItem(ItemRequest itemRequest)
        {
            ItemResponse itemResponse = new ItemResponse();
            List<Item> _itemList;
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
                _itemList = _itemDbContext.UpdateItem(itemRequest);
                if ((_itemList.FindAll(i => i.Id == itemRequest.Id && i.Name == itemRequest.Name)).Any())
                {
                    itemResponse.Status = Status.Success;
                    itemResponse.Items = _itemList;
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
                    if (item.isActive == false)
                        return true;
                }
            }
            return false;
        }
    }
}
