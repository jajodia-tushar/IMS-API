using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxLimit { get; set; }
        public bool IsActive { get; set; }
    }
}
