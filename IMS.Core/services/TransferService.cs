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
            throw new NotImplementedException();
        }

        public async Task<Response> TransferToWarehouse(int OrderId)
        {
            throw new NotImplementedException();
        }
    }
}
