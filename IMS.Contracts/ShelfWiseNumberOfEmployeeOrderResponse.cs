using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfWiseNumberOfEmployeeOrderResponse:Response
    {
        public Dictionary<string, List<ShelfEmployeeOrderCountMapping>> ShelfWiseEmployeeOrderOfEachDay;
    }
}
