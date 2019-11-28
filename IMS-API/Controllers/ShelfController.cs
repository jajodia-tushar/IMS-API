using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IMS.Contracts;
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
        public List<Shelf> Get()
        {
            IMS.Contracts.ShelfResponse contractsShelfResponse;
            IMS.Entities.ShelfResponse entityShelfResponse = _shelfService.GetShelfList();
            contractsShelfResponse = Translator.ToDataContractsObject(entityShelfResponse);
            List<Shelf> shelfList = contractsShelfResponse.GetShelves;

            return shelfList;
           
        }

        // GET: api/Shelf/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Shelf
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
