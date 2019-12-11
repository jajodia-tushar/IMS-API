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
    public class TransferController : ControllerBase
    {
        private ITransferService _transferService;
        private ILogManager _logger;
        public TransferController(ITransferService transferService, ILogManager logManager)
        {
            this._transferService = transferService;
            this._logger = logManager;
        }
        /// <summary>
        /// Transfer items from warehouse to shelf
        /// </summary>
        /// <param name="transferRequest">The list of items and shelves to which transfer has to be made</param>
        /// <returns>Status</returns>
        /// <response code="200">Returns Success if transfer is successfull else returns status failure</response>
        // Patch: api/TransferToShelves
        [HttpPatch("TransferToShelves")]
        public async Task<Response> TransferToShelf([FromBody] TransferToShelvesRequest transferRequest)
        {
            Response transferResponse = null;
            try
            {
                IMS.Entities.TransferToShelvesRequest entityTransferRequest = TransferTranslator.ToEntitiesObject(transferRequest);
                IMS.Entities.Response entityTransferResponse = await _transferService.TransferToShelves(entityTransferRequest);
                transferResponse = Translator.ToDataContractsObject(entityTransferResponse);
            }
            catch (Exception exception)
            {
                transferResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Transfer to shelves", IMS.Entities.Severity.Critical, transferRequest, transferResponse); }).Start();
            }
            return transferResponse;
        }
    }
}
