using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
   public class ShelfWiseNumberOfEmployeeOrderResponse:Response
   {
        public Dictionary<string, List<ShelfEmployeeOrderCountMapping>> ShelfWiseEmployeeOrderOfEachDay;
   }
}
