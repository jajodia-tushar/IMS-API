using IMS.Core.Validators;
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
                        if (TransferValidator.ValidateTransferToShelvesRequest(transferToShelvesRequest) == false)
                        {
                            transferToShelvesResponse.Status = Status.Failure;
                            transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.MissingValues);
                            return transferToShelvesResponse;
                        }
                        bool responseStatus = await _transferDbContext.TransferToShelves(transferToShelvesRequest);
                        if (responseStatus == true)
                        {
                            transferToShelvesResponse.Status = Status.Success;
                        }
                        else
                        {
                            transferToShelvesResponse.Status = Status.Failure;
                            transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.TranferFailure);
                        }
                    }
                    catch (Exception exception)
                    {
                        new Task(() => { _logger.LogException(exception, "Transfer to shelves", IMS.Entities.Severity.Critical, transferToShelvesRequest, transferToShelvesResponse); }).Start();
                    }
                    return transferToShelvesResponse;
                }
                else
                {
                    transferToShelvesResponse.Status = Status.Failure;
                    transferToShelvesResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "Transfer to shelves", IMS.Entities.Severity.Critical, transferToShelvesRequest, transferToShelvesResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (transferToShelvesResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(transferToShelvesRequest, transferToShelvesResponse, "Transfer to shelf", transferToShelvesResponse.Status, severity, userId); }).Start();
            }
            return transferToShelvesResponse;
        }
    }
}
