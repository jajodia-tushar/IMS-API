using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpGet]
        public Object Do()
        {
           return  _inventoryService.TestMethod();
        }
        //Get Inventory By shelf Id

        //POST: Create a new Category/Item

        //DELETE: soft delete
    }
}