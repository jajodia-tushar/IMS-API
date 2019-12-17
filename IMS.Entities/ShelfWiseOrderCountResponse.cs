using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
   public class ShelfWiseOrderCountResponse: Response
   {
        public Dictionary<DateTime, List<ShelfOrderCountMapping>> DateWiseShelfOrderCount;
   }
}
