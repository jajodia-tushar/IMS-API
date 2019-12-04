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
        // GET: api/Shelf
        [HttpGet]
        public ShelfResponse Get()
        {
            IMS.Contracts.ShelfResponse contractsShelfResponse;
            IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.GetShelfList();
            contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            return contractsShelfResponse;
          
           
        }

        // GET: api/Shelf/5
        [HttpGet("{id}", Name = "GetShelf")]
        public ShelfResponse Get(string id)
        {
            IMS.Contracts.ShelfResponse contractsShelfResponse;
            IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.GetShelfById(id);
            contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            return contractsShelfResponse;
            
        }

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

        // DELETE: api/Shelf/B
        [HttpDelete("{id}")]
        public ShelfResponse Delete(string id)
        {
            ShelfResponse contractShelfResponse = null;
            try
            {

                IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.Delete(id);
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
