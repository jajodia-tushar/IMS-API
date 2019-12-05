using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ShelfItemResponse : Response
    {
        public Shelf shelf { get; set; }
        public List<ItemQuantityMapping> itemQuantityMapping { get; set; }
    }
}
