using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class TransferToShelvesRequest
    {
        List<TransferToShelfRequest> ShelvesItemsQuantityList { get; set; }
    }
}
