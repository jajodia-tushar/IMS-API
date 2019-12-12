using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class TransferService : ITransferService
    {
        private ITransferDbContext _transferDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;

        public TransferService(ITransferDbContext transferDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._transferDbContext = transferDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider; 
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response> TransferToShelves(TransferToShelvesRequest transferToShelvesRequest)
        {
            Response transferToShelvesResponse = new Response();
            try
            {
                if (checkIfAnyValueIsNull(transferToShelvesRequest))
                {
                    transferToShelvesResponse.Status = Status.Failure;
                    transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.MissingValues);
                    return transferToShelvesResponse;
                }
                Status responseStatus =await _transferDbContext.TransferToShelves(transferToShelvesRequest);
                if (responseStatus == Status.Success)
                {
                    transferToShelvesResponse.Status = Status.Success;
                }
                else
                {
                    transferToShelvesResponse.Status = Status.Failure;
                    transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.TranferFailure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Severity severity = Severity.No;
                if (transferToShelvesResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(transferToShelvesRequest, transferToShelvesResponse, "Transfer to shelf", transferToShelvesResponse.Status, severity, -1); }).Start();
            }
            return transferToShelvesResponse;
        }

        private bool checkIfAnyValueIsNull(TransferToShelvesRequest transferToShelvesRequest)
        {
            foreach(TransferToShelfRequest transferToShelfRequest in transferToShelvesRequest.ShelvesItemsQuantityList)
            {
                if(transferToShelfRequest.Shelf.Id==0)
                {
                    return true;
                }
                foreach(ItemQuantityMapping itemQuantityMapping in transferToShelfRequest.ItemQuantityMapping)
                {
                    if(itemQuantityMapping.Item.Id==0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<Response> TransferToWarehouse(int OrderId)
        {
            Response response = new Response();
            try
            {
                bool isTransferred = await _transferDbContext.TransferToWarehouse(OrderId);
                if (isTransferred)
                {
                    response.Status = Status.Success;
                }
                else
                {
                    response.Status = Status.Failure;
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.TranferFailure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
    }
}
