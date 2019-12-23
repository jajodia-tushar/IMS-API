using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class TransferToShelvesRequest
    {
        public List<TransferToShelfRequest> ShelvesItemsQuantityList { get; set; }
    }
}
