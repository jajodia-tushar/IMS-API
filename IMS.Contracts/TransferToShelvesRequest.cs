using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class TransferToShelvesRequest
    {
        public List<TransferToShelfRequest> ShelvesItemsQuantityList { get; set; }
    }
}
