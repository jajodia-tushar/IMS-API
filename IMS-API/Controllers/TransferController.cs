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
    public class TransferController : ControllerBase
    {
        private ITransferService _transferService;
        public TransferController(ITransferService transferService)
        {
            this._transferService = transferService;
        }
        /// <summary>
        /// Transfer items from warehouse to shelf
        /// </summary>
        /// <param name="transferToShelvesRequest">The list of items and shelves to which transfer has to be made</param>
        /// <returns>Status</returns>
        /// <response code="200">Returns Success if tranfser is successfull else returns status failure</response>
        // Patch: api/TransferToShelves
        [HttpPatch("TransferToShelves")]
        public async Task<Response> TransferToShelf([FromBody] TransferToShelvesRequest transferToShelvesRequest)
        {
            Response transferToShelfResponse = null;
            try
            {
                IMS.Entities.TransferToShelvesRequest entityTransferToShelvesRequest = TransferTranslator.ToEntitiesObject(transferToShelvesRequest);
                IMS.Entities.Response entityTransferToShelfResponse = await _transferService.TransferToShelves(entityTransferToShelvesRequest);
                transferToShelfResponse = TransferTranslator.ToDataContractsObject(entityTransferToShelfResponse);
            }
            catch
            {
                transferToShelfResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return transferToShelfResponse;
        }

        /// <summary>
        /// Transfer items to warehouse after Admin approval
        /// </summary>
        /// <param name="orderId">Here OrderId represents Id of that particular Order</param>
        /// <returns>Response</returns>
        /// <response code="200">Returns Success if order is Updated To Warehouse Successfully otherwise returns Status Failure</response>
        // Patch: api/TransferToWarehouse
        [HttpPatch("{orderId}",Name ="TransferToWarehouse")]
        public async Task<Response> TransferToWarehouse( int orderId)
        {
            Response transferToWarehouseResponse = null;
            try
            {
                IMS.Entities.Response entityTransferWarehouseResponse = await _transferService.TransferToWarehouse(orderId);
                transferToWarehouseResponse = TransferTranslator.ToDataContractsObject(entityTransferWarehouseResponse);
            }
            catch
            {
                transferToWarehouseResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
            }
            return transferToWarehouseResponse;
        }
   
    }
}
