using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class Shelf
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}
