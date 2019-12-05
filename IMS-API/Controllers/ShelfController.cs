using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfController : ControllerBase
    {

        private IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
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
            IMS.Contracts.ShelfResponse contractsShelfResponse;
            IMS.Entities.ShelfResponse entityShelfResponse = await _shelfService.GetShelfList();
            contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            return contractsShelfResponse;
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
        public ShelfResponse Get(string shelfCode)
        {
            IMS.Contracts.ShelfResponse contractsShelfResponse;
            IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.GetShelfByShelfCode(shelfCode);
            contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            return contractsShelfResponse;
            
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
        public ShelfResponse Post([FromBody] Shelf shelf)
        {
           ShelfResponse contractsShelfResponse = null;
            try
            {
                IMS.Entities.Shelf entityShelf = Translator.ToEntitiesObject(shelf);
                IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.AddShelf(entityShelf);
                contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            }
            catch
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
            }
            return contractsShelfResponse;
        }

        // PUT: api/Shelf/5
        [HttpPut("{id}")]
        public void put(int id)
        {
           
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
        public ShelfResponse DeleteShelfByShelfCode(string shelfCode)
        {
            ShelfResponse contractShelfResponse = null;
            try
            {

                IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.Delete(shelfCode);
                contractShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);

            }
            catch
            {
                contractShelfResponse = new IMS.Contracts.ShelfResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return contractShelfResponse;
        }
    }
}
