using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class TransferToShelvesRequest
    {
        List<TransferToShelfRequest> ShelvesItemsQuantityList { get; set; }
    }
}
