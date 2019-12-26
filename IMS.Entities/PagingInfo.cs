using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class PagingInfo
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int totalResults { get; set; }
    }
}
