using IMS.Entities;
using IMS.DataLayer.Interfaces;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class InventoryService:IInventoryService
    {
        private IInventoryDbContext _inventoryDbContext;

        public InventoryService(IInventoryDbContext inventoryDbContext)
        {
            _inventoryDbContext = inventoryDbContext;
        }

        public ShelfItemsResponse GetShelfItemsByShelfId(int shelfId)
        {
            ShelfItemsResponse shelfItemsResponse = new ShelfItemsResponse();
            shelfItemsResponse.Status = Status.Failure;
            try
            {
                //Check here if given shelf exists or not if exists get the shelf details

                shelfItemsResponse.itemQuantityMappings = _inventoryDbContext.GetShelfItemsByShelfId(shelfId);
                if (shelfItemsResponse.itemQuantityMappings.Count > 0)
                {
                    shelfItemsResponse.Status = Status.Success;
                    return shelfItemsResponse;
                }
                shelfItemsResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.NotFound,
                    ErrorMessage = Constants.ErrorMessages.EmptyShelf
                };
                return shelfItemsResponse;

                /*shelfItemsResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.BadRequest,
                    ErrorMessage = Constants.ErrorMessages.InvalidId
                };*/
            }
            catch(Exception e)
            {
                throw e;
            }
            return shelfItemsResponse;
        }
    }
}
