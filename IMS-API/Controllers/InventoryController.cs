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

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            this._inventoryService = inventoryService;
        }

        // GET: api/Inventory/5
        [HttpGet("{shelfId}", Name = "GetShelfItemsByShelfId")]
        public ShelfItemsResponse GetShelfItemsByShelfId(int shelfId)
        {
            ShelfItemsResponse shelfItemsResponse = null;
            try
            {
                IMS.Entities.ShelfItemsResponse shelfItemsResponseEntity = _inventoryService.GetShelfItemsByShelfId(shelfId);
                shelfItemsResponse = Translator.ToDataContractsObject(shelfItemsResponseEntity);
            }
            catch(Exception e)
            {
                shelfItemsResponse = new IMS.Contracts.ShelfItemsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return shelfItemsResponse;
        }

        // POST: api/Inventory
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Inventory/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
