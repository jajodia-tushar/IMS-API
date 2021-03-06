using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private IItemService _itemService;
        private ILogManager _logger;

        public ItemController(IItemService itemService, ILogManager logManager)
        {
            _itemService = itemService;
            this._logger = logManager;
        }

        /// <summary>
        /// Return All The Items Present In Inventory
        /// </summary>
        /// <returns>Details of All Items</returns>
        /// <response code="200">Returns All The Items From Inventory</response>
        // GET: api/Item
        [HttpGet]
        public async Task<ItemResponse> Get()
        {
            ItemResponse contractItemsResponse = null;
            try
            {
                IMS.Entities.ItemResponse entityItemResponse = await _itemService.GetAllItems();
                contractItemsResponse = ItemTranslator.ToDataContractsObject(entityItemResponse);
            }
            catch(Exception exception)
            {
                contractItemsResponse = new IMS.Contracts.ItemResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Get", IMS.Entities.Severity.Critical, null, contractItemsResponse); }).Start();
            }
            return contractItemsResponse;
        }
        
        /// <summary>
        /// Retrieve The Item By Their ID
        /// </summary>
        /// <param name="id">The ID of that Item</param>
        /// <returns>Details of Item</returns>
        /// <response code="200">Returns Item Details if Item Id is valid otherwise it returns null with status failure</response>
        // GET: api/Item/5
        [HttpGet("{id}", Name = "GetItem")]
        public async Task<ItemResponse> Get(int id)
        {
            ItemResponse contractItemsResponse = null;
            try
            {
                IMS.Entities.ItemResponse entityItemResponse = await _itemService.GetItemById(id);
                contractItemsResponse = ItemTranslator.ToDataContractsObject(entityItemResponse);
            }
            catch (Exception exception)
            {
                contractItemsResponse = new IMS.Contracts.ItemResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetItemById", IMS.Entities.Severity.Critical, id, contractItemsResponse); }).Start();
            }
            return contractItemsResponse;
        }
        
        /// <summary>
        /// Creates A New Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return All Items Along With Newly Created Item </returns>
        /// <response code="200">Returns Items List along with newly created Item if Item is added successfully otherwise it returns null with status failure</response>
        // POST: api/Item
        [HttpPost]
        public async Task<ItemResponse> Add([FromBody]  Item item)
        {
            ItemResponse contractItemsResponse = null;
            try
            {
                IMS.Entities.Item entityItemRequest = ItemTranslator.ToEntitiesObject(item);
                IMS.Entities.ItemResponse entityItemResponse = await _itemService.AddItem(entityItemRequest);
                contractItemsResponse = ItemTranslator.ToDataContractsObject(entityItemResponse);
            }
            catch(Exception exception)
            {
                contractItemsResponse = new IMS.Contracts.ItemResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Add", IMS.Entities.Severity.Critical, item, contractItemsResponse); }).Start();
            }
            return contractItemsResponse;
        }
        
        /// <summary>
        /// Update the Specific Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns Item List along with updtaed Item</returns>
        /// <response code="200">Returns Item List along with updated Item if Item is updated successfully otherwise it returns null with status failure</response>
        // PATCH: api/Item
        [HttpPut]
        public async Task<ItemResponse> Update([FromBody] Item item)
        {
            ItemResponse contractItemsResponse = null;
            try
            {
                IMS.Entities.Item entityItemRequest = ItemTranslator.ToEntitiesObject(item);
                IMS.Entities.ItemResponse entityItemResponse = await _itemService.UpdateItem(entityItemRequest);
                contractItemsResponse = ItemTranslator.ToDataContractsObject(entityItemResponse);
            }
            catch(Exception exception)
            {
                contractItemsResponse = new IMS.Contracts.ItemResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Update", IMS.Entities.Severity.Critical, item, contractItemsResponse); }).Start();
            }
            return contractItemsResponse;
        }

        /// <summary>
        /// Deactivate the specific Item
        /// </summary>
        /// <param name="id">The ID of that Item</param>
        /// <param name="isHardDelete">give boolean value wether item should be har delete or soft delete</param>
        /// <response code="200">Returns Item List along with Deactivated Item if that Item is softdeleted successfully otherwise it returns null with status failure</response>
        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<ItemResponse> Delete(int id,bool isHardDelete)
        {
            ItemResponse contractItemsResponse = null;
            try
            {
                IMS.Entities.ItemResponse entityItemResponse = await _itemService.Delete(id,isHardDelete);
                contractItemsResponse = ItemTranslator.ToDataContractsObject(entityItemResponse);
            }
            catch(Exception exception)
            {
                contractItemsResponse = new IMS.Contracts.ItemResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Delete", IMS.Entities.Severity.Critical, id, contractItemsResponse); }).Start();
            }
            return contractItemsResponse;
        }
    }
}
