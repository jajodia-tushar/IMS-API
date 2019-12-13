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
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        if (checkIfAnyValueIsNull(transferToShelvesRequest))
                        {
                            transferToShelvesResponse.Status = Status.Failure;
                            transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.MissingValues);
                            return transferToShelvesResponse;
                        }
                        Status responseStatus = await _transferDbContext.TransferToShelves(transferToShelvesRequest);
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
                    catch(Exception e)
                    {
                        throw e;
                    }
                    return transferToShelvesResponse;
                }
                else
                {
                    transferToShelvesResponse.Status = Status.Failure;
                    transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
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
        public async Task<Response> TransferToWarehouse(int orderId)
        {
            Response transferToWarehouseResponse = new Response();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        if(orderId == 0)
                        {
                            transferToWarehouseResponse.Status = Status.Failure;
                            transferToWarehouseResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidOrderId);
                        }
                        else
                        {
                            bool isTransferred = await _transferDbContext.TransferToWarehouse(orderId);
                            if (isTransferred)
                                transferToWarehouseResponse.Status = Status.Success;
                            else
                            {
                                transferToWarehouseResponse.Status = Status.Failure;
                                transferToWarehouseResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.TranferFailure);
                            }
                        } 
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return transferToWarehouseResponse;
                }
                else
                {
                    transferToWarehouseResponse.Status = Status.Failure;
                    transferToWarehouseResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch
            {
                transferToWarehouseResponse.Status = Status.Failure;
                transferToWarehouseResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            finally
            {
                Severity severity = Severity.No;
                if (transferToWarehouseResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(orderId, transferToWarehouseResponse, "TransferToWarehouse", transferToWarehouseResponse.Status, severity, userId); }).Start();
            }
            return transferToWarehouseResponse;
        }
    }
}
