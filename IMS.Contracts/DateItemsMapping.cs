using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class DateItemsMapping
    {
        public string Date { get; set; }
        public List<ItemQuantityMapping> ItemQuantityMappings { get; set; }
    }
}
