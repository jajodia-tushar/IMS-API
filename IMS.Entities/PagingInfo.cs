using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class PagingInfo
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalResults { get; set; }
    }
}
