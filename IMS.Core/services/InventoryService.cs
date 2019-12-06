using IMS.Entities;
using IMS.DataLayer.Interfaces;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class InventoryService:IInventoryService
    {
        private IInventoryDbContext _inventoryDbContext;
        private IShelfService _shelfService;
        public InventoryService(IInventoryDbContext inventoryDbContext,IShelfService shelfService)
        {
            _inventoryDbContext = inventoryDbContext;
            _shelfService = shelfService;
        }

        public async Task<ShelfItemsResponse> GetShelfItemsByShelfCode(string shelfCode)
        {
            ShelfItemsResponse shelfItemsResponse = new ShelfItemsResponse()
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
                    ShelfResponse shelfResponse = await _shelfService.GetShelfByShelfCode(shelfCode);
                    if (shelfResponse.Shelves!=null)
                    {
                        shelfItemsResponse.shelf = shelfResponse.Shelves[0];
                        shelfItemsResponse.itemQuantityMappings = _inventoryDbContext.GetShelfItemsByShelfCode(shelfResponse.Shelves[0].Id);
                        if (shelfItemsResponse.itemQuantityMappings.Count>0)
                        {
                            shelfItemsResponse.Status = Status.Success;
                            shelfItemsResponse.Error = null;
                            return shelfItemsResponse;
                        }
                        shelfItemsResponse.Error.ErrorCode = Constants.ErrorCodes.NotFound;
                        shelfItemsResponse.Error.ErrorMessage = Constants.ErrorMessages.EmptyShelf;
                        return shelfItemsResponse;
                    }
                    shelfItemsResponse.Error.ErrorCode = Constants.ErrorCodes.NotFound;
                    shelfItemsResponse.Error.ErrorMessage = Constants.ErrorMessages.ShelfNotPresent;
                    return shelfItemsResponse;
                }
                catch(Exception exception)
                {
                    throw exception;
                }
            }
            return shelfItemsResponse;
        }
    }
}
