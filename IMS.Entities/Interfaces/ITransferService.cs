using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface ITransferService
    {
        Task<Response> TransferToShelves(TransferToShelvesRequest transferToShelvesRequest);
    }
}
