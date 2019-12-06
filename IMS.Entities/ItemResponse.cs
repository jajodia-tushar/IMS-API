using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ItemResponse : Response
    {
        public List<Item> Items { get; set; }
    }
}
