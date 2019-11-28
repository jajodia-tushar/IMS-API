using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfResponse : Response
    {
        public List<Shelf> GetShelves { get; set; }
        public Shelf Shelf { get; set; }
    }
}
