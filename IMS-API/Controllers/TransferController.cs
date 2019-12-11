using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private ITransferService _transferService;
        public TransferController(ITransferService transferService)
        {
            this._transferService = transferService;
        }
        // Patch: api/TransferToShelves
        [HttpPatch("TransferToShelves")]
        public Response TransferToShelf([FromBody] TransferToShelvesRequest transferToShelvesRequest)
        {
            Response response = new Response();
            return response;
        }

        // Patch: api/TransferToWarehouse

    }
}
