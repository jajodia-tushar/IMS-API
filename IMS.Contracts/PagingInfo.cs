using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class PagingInfo
    {
        public int CurrentPageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalEntries { get; set; }
    }
}
