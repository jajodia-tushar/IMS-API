using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface ITransferDbContext
    {
        Task<Status> TransferToWarehouse(int OrderId);
        Task<Status> TransferToShelves(TransferToShelvesRequest transferToShelvesRequest);
    }
}
