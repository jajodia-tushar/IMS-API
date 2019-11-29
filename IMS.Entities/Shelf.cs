using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class Shelf
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool isActive { get; set; }
    }
}
