using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ShelfItemsResponse:Response
    {
        public Shelf Shelf;
        public List<ItemQuantityMapping> ItemQuantityMappings;
    }
}
