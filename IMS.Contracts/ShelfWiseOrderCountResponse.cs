using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfWiseOrderCountResponse:Response
    {
        public Dictionary<DateTime, List<ShelfOrderCountMapping>> DateWiseShelfOrderCount;
    }
}
