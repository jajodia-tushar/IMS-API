using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfController : ControllerBase
    {

        private IShelfService _shelfService;
        private ILogManager _logger;

        public ShelfController(IShelfService shelfService, ILogManager logManager)
        {
            _shelfService = shelfService;
            this._logger = logManager;
        }
        /// <summary>
        /// Getting all Shelf List 
        /// </summary>
        /// <remarks>
        /// Note that it doesn't contain any data
        /// 
        ///     GET 
        ///     {
        ///     }
        ///     
        /// </remarks>
        /// <returns>All Shelves List</returns>
        /// <response code="200">Returns shelves List </response>
        /// <response code="404">If shelves list is empty </response>
        // GET: api/Shelf
        [HttpGet]
        public async Task<ShelfResponse> Get()
        {
            IMS.Contracts.ShelfResponse dtoShelfResponse;
            try
            {
                IMS.Entities.ShelfResponse doShelfResponse = await _shelfService.GetShelfList();
                dtoShelfResponse = ShelfTranslator.ToDataContractsObject(doShelfResponse);
            }
            catch (Exception exception)
            {
                dtoShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetShelfList", IMS.Entities.Severity.High,"", dtoShelfResponse); }).Start();
            }
            return dtoShelfResponse;
        }
        /// <summary>
        /// Getting Shelf by it's Code and return shelf Object
        /// </summary>
        /// <remarks>
        /// Note that shelf data contains shelfCode 
        /// 
        ///     GET 
        ///     {
        ///        "Code":"string"
        ///     }
        ///     
        /// </remarks>
        /// <returns>Shelf Object</returns>
        /// <response code="200">Returns shelf Object </response>
        /// <response code="404">If shelf Code is Invalid </response>
        // GET: api/Shelf/5
        [HttpGet("{shelfCode}", Name = "GetShelf")]
        public async Task<ShelfResponse> Get(string shelfCode)
        {
            IMS.Contracts.ShelfResponse dtoShelfResponse;
            try
            {
                IMS.Entities.ShelfResponse doShelfResponse = await _shelfService.GetShelfByShelfCode(shelfCode);
                dtoShelfResponse = ShelfTranslator.ToDataContractsObject(doShelfResponse);
            }
            catch (Exception exception)
            {
                dtoShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetShelfByShelfCode", IMS.Entities.Severity.High,shelfCode, dtoShelfResponse); }).Start();
            }
            return dtoShelfResponse;     
        }

        /// <summary>
        /// Adding new Shelf and return List of shelves
        /// </summary>
        /// <remarks>
        /// Note that new shelf data contains shelfId, shelfName and shelfCode 
        /// 
        ///     POST 
        ///     {
        ///        "Id":"int",
        ///        "Name":"string",
        ///        "IsActive":"bool",
        ///        "Code":"string"
        ///        
        ///     }
        ///     
        /// </remarks>
        /// <returns>All Shelves List</returns>
        /// <response code="200">Returns shelves List </response>
        /// <response code="400">If shelf is Already present </response>

        // POST: api/Shelf
        [HttpPost]
        public async Task< ShelfResponse> Post([FromBody] Shelf shelf)
        {
           ShelfResponse dtoShelfResponse = null;
            try
            {
                IMS.Entities.Shelf doShelf = ShelfTranslator.ToEntitiesObject(shelf);
                IMS.Entities.ShelfResponse doShelfResponse =await _shelfService.AddShelf(doShelf);
                dtoShelfResponse = ShelfTranslator.ToDataContractsObject(doShelfResponse);
            }
            catch(Exception exception)
            {
                dtoShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Post", IMS.Entities.Severity.High, shelf, dtoShelfResponse); }).Start();
            }
            return dtoShelfResponse;
        }

        /// <summary>
        /// Update the Specific Shelf
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns>Returns Updated Shelf</returns>
        /// <response code="200">Returns Updated Shelf if Shlef is updated successfully otherwise it returns null with status failure</response>
        [HttpPut]
        public async Task<ShelfResponse> Update(Shelf shelf)
        {
            ShelfResponse contractsShelfResponse = new ShelfResponse();
            try
            {
                IMS.Entities.Shelf entityShelfRequest = ShelfTranslator.ToEntitiesObject(shelf);
                IMS.Entities.ShelfResponse entityShelfResponse = await _shelfService.Update(entityShelfRequest);
                contractsShelfResponse = ShelfTranslator.ToDataContractsObject(entityShelfResponse);
            }
            catch(Exception exception)
            {
                contractsShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Update", IMS.Entities.Severity.High, shelf, contractsShelfResponse); }).Start();
            }
            return contractsShelfResponse;
        }

        /// <summary>
        /// Deleting Shelf by it's Code and return List of remaining shelves
        /// </summary>
        /// <remarks>
        /// Note that shelf data contains shelfCode 
        /// 
        ///     DELETE
        ///     {
        ///        "Code":"string"
        ///     }
        ///     
        /// </remarks>
        /// <returns>All Shelves List</returns>
        /// <response code="200">Returns remaining shelves List </response>
        /// <response code="400">If shelf is Already Deleted or Not Present in Shelf Table </response>


        // DELETE: api/Shelf/B
        [HttpDelete("{shelfCode}")]
        public async Task< ShelfResponse> DeleteShelfByShelfCode(string shelfCode)
        {
            ShelfResponse dtoShelfResponse = null;
            try
            {

                IMS.Entities.ShelfResponse doShelfResponse = await _shelfService.Delete(shelfCode);
                dtoShelfResponse = ShelfTranslator.ToDataContractsObject(doShelfResponse);

            }
            catch(Exception exception)
            {
                dtoShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "DeleteShelfByShelfCode", IMS.Entities.Severity.High, shelfCode, dtoShelfResponse); }).Start();
            }
            return dtoShelfResponse;
        }
    }
}
