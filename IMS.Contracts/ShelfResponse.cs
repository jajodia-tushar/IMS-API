using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ShelfResponse : Response
    {
        public List<Shelf> shelfList { get; set; }
       
    }
}
