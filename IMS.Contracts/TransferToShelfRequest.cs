using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class TransferToShelfRequest
    {
        public Shelf Shelf { get; set; }
        public ItemQuantityMapping ItemQuantityMapping { get; set; }
    }
}

