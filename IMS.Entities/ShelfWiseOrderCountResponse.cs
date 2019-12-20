using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
   public class ShelfWiseOrderCountResponse: Response
   {
        public List<ShelfOrderStats> DateWiseShelfOrderCount { get; set; }
   }
}
