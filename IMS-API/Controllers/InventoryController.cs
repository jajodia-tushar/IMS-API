using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Core.Translators;
using IMS.Contracts;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IMS.Core;
using IMS.Logging;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryService _inventoryService;
        private ILogManager _logger;
        public InventoryController(IInventoryService inventoryService,ILogManager logManager)
        {
            this._inventoryService = inventoryService;
            this._logger = logManager;
        }

        /// <summary>
        /// Retrieve Shelf Items and Shelf Details by Shelf Code
        /// </summary>
        /// <param name="shelfCode">Unique Code of the shelf</param>
        /// <returns>Items present in Particular Shelf</returns>
        /// <response code="200">Return Items Present in shelf along with shelf details if shelf code is valid otherwise it returns null with status failure </response>
        // GET: api/Inventory/5
        [HttpGet("{shelfCode}", Name = "Get")]
        public async Task<ShelfItemsResponse> Get(string shelfCode)
        {
            ShelfItemsResponse shelfItemsResponse = new IMS.Contracts.ShelfItemsResponse()
            {
                Status = Status.Failure,
                Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.BadRequest,
                    ErrorMessage = Constants.ErrorMessages.InvalidId
                }
            };

            if (!String.IsNullOrEmpty(shelfCode))
            {
                try
                {
                    int x = Convert.ToInt32("Rajat");
                    IMS.Entities.ShelfItemsResponse shelfItemsResponseEntity = await _inventoryService.GetShelfItemsByShelfCode(shelfCode);
                    return Translator.ToDataContractsObject(shelfItemsResponseEntity);
                }
                catch (Exception exception)
                {
                    shelfItemsResponse.Error.ErrorCode = Constants.ErrorCodes.ServerError;
                    shelfItemsResponse.Error.ErrorMessage = Constants.ErrorMessages.ServerError;
                    _logger.LogException(exception, shelfCode, shelfItemsResponse);
                }
            }
            return shelfItemsResponse;
        }
    }
}
