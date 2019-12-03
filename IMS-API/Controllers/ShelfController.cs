﻿using System;
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
        [HttpGet("{id}", Name = "Get")]
        public ShelfResponse Get(int id)
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
                        ErrorMessage = Constants.ErrorMessages.ShelfIsAlreadyPresent
                    }
                };
            }
            return contractsShelfResponse;
        }

        // PUT: api/Shelf/5
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
