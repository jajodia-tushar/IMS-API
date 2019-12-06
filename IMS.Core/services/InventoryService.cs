using IMS.Entities;
using IMS.DataLayer.Interfaces;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IMS.Logging;

namespace IMS.Core.services
{
    public class InventoryService:IInventoryService
    {
        private IInventoryDbContext _inventoryDbContext;
        private IShelfService _shelfService;
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogManager _logger;
        public InventoryService(IInventoryDbContext inventoryDbContext,IShelfService shelfService, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, ILogManager logger)
        {
            _inventoryDbContext = inventoryDbContext;
            _shelfService = shelfService;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    if (!String.IsNullOrEmpty(shelfCode))
                    {
                        try
                        {
                            ShelfResponse shelfResponse = await _shelfService.GetShelfByShelfCode(shelfCode);
                            if (shelfResponse.Shelves != null)
                            {
                                shelfItemsResponse.shelf = shelfResponse.Shelves[0];
                                shelfItemsResponse.itemQuantityMappings = await _inventoryDbContext.GetShelfItemsByShelfCode(shelfResponse.Shelves[0].Id);
                                if (shelfItemsResponse.itemQuantityMappings.Count > 0)
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
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                    }
                    return shelfItemsResponse;
                }
                else
                {
                    shelfItemsResponse.Status = Status.Failure;
                    shelfItemsResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.UnAuthorized,
                        ErrorMessage = Constants.ErrorMessages.InvalidToken
                    };
                }
            }
            catch
            {
                shelfItemsResponse.Status = Status.Failure;
                shelfItemsResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
            }
            finally
            {
                Severity severity = Severity.No;
                if (shelfItemsResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(shelfCode, shelfItemsResponse, "GetShelfItemsByShelfCode", shelfItemsResponse.Status, severity, -1); }).Start();
            }
            return shelfItemsResponse;
        }
    }
}
