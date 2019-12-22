using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemResponse : Response
    {
        public List<Item> Items { get; set; }
    }
}
