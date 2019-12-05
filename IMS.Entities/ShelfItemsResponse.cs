using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ShelfItemsResponse:Response
    {
        public Shelf shelf;
        public List<ItemQuantityMapping> itemQuantityMappings;
    }
}
