using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class PagingInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
    }
}
