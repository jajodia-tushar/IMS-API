using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
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
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogManager _logger;
        public ItemService(IItemDbContext itemDbContext, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, ILogManager logger)
        {
            _itemDbContext = itemDbContext;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ItemResponse> GetAllItems()
        {
            ItemResponse itemResponse = new ItemResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    List<Item> itemList;
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
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.resourceNotFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                itemResponse.Status = Status.Failure;
                itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(null, itemResponse, "GetAllItems", itemResponse.Status, severity, userId); }).Start();
            }
            return itemResponse;
        }

        public async Task<ItemResponse> GetItemById(int id)
        {
            ItemResponse itemResponse = new ItemResponse();
            int userId = -1;
            try
            {
                List<Item> itemList = new List<Item>();
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        Item item = await _itemDbContext.GetItemById(id);
                        if (item.Id.Equals(id))
                        {
                            itemResponse.Status = Status.Success;
                            itemList.Add(item);
                            itemResponse.Items = itemList;
                        }
                        else
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnprocessableEntity,Constants.ErrorMessages.resourceNotFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return itemResponse;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized,Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                itemResponse.Status = Status.Failure;
                itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(id, itemResponse, "GetItemById", itemResponse.Status, severity, userId); }).Start();
            }
            return itemResponse;
        }

        public async Task<ItemResponse> AddItem(Item item)
        {
            ItemResponse itemResponse = new ItemResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        if (string.IsNullOrEmpty(item.Name))
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidItemsDetails);
                            return itemResponse;
                        }
                        if (await IsItemAlreadyExists(item.Name))
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.AlreadyPresent);
                            return itemResponse;
                        }
                        int latestAddedItemId = await _itemDbContext.AddItem(item);
                        Item createdItem = await _itemDbContext.GetItemById(latestAddedItemId);
                        if (createdItem.Name.Equals(item.Name))
                        {
                            itemResponse.Status = Status.Success;
                            itemResponse.Items = await _itemDbContext.GetAllItems();
                        }
                        else
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.Conflict);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return itemResponse;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                itemResponse.Status = Status.Failure;
                itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(item, itemResponse, "AddItem", itemResponse.Status, severity, userId); }).Start();
            }
            return itemResponse;
        }

        public async Task<ItemResponse> Delete(int id)
        {
            ItemResponse itemResponse = new ItemResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        if (await IsItemAlreadyDeleted(id))
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.AlreadyDeleted);
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
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NotDeleted);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return itemResponse;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                itemResponse.Status = Status.Failure;
                itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(id, itemResponse, "Delete", itemResponse.Status, severity, userId); }).Start();
            }
            return itemResponse;
        }

        public async Task<ItemResponse> UpdateItem(Item item)
        {
            ItemResponse itemResponse = new ItemResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        if (string.IsNullOrEmpty(item.Name))
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidItemsDetails);
                            return itemResponse;
                        }
                        Item updatedItem = await _itemDbContext.UpdateItem(item);
                        if (updatedItem.Id.Equals(item.Id) && updatedItem.Name.Equals(item.Name) && updatedItem.MaxLimit.Equals(item.MaxLimit))
                        {
                            itemResponse.Status = Status.Success;
                            itemResponse.Items = await _itemDbContext.GetAllItems();
                        }
                        else
                        {
                            itemResponse.Status = Status.Failure;
                            itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.NotUpdated);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return itemResponse;
                }
                else
                {
                    itemResponse.Status = Status.Failure;
                    itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                itemResponse.Status = Status.Failure;
                itemResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(item, itemResponse, "UpdateItem", itemResponse.Status, severity, userId); }).Start();
            }
            return itemResponse;
        }

        private async Task<bool> IsItemAlreadyExists(string name)
        {
            List<Item> items = await _itemDbContext.GetAllItems();
            foreach (var item in items)
            {
                if (item.Name.Equals(name))
                    return true;
            }
            return false;
        }

        private async Task<bool> IsItemAlreadyDeleted(int id)
        {
            List<Item> items = await _itemDbContext.GetAllItems();
            foreach (var item in items)
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
