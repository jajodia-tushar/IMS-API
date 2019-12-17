using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class MostConsumedItemsResponse : Response
    {
        public List<ItemQuantityMapping> ItemQuantityMapping;
    }
}
