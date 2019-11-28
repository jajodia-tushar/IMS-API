using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ShelfResponse : Response
    {
        public Shelf Shelf { get; set; }
        public List<Shelf> GetShelves{get ; set; }
    }
}
