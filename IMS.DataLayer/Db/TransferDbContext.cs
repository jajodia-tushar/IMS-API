using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class TransferDbContext : ITransferDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public TransferDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<Status> TransferToShelves(TransferToShelvesRequest transferToShelvesRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<Status> TransferToWarehouse(int OrderId)
        {
            throw new NotImplementedException();
        }
    }
}
