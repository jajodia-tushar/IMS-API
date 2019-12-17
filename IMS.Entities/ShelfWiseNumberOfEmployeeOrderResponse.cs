using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
   public class ShelfWiseNumberOfEmployeeOrderResponse:Response
   {
        public Dictionary<DateTime, List<ShelfEmployeeOrderCountMapping>> ShelfWiseEmployeeOrderOfEachDay;
   }
}
