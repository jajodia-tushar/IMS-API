using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ReportsService : IReportsService
    {
        public Task<RAGStatusResponse> GetRAGStatus()
        {
            throw new NotImplementedException();
        }

        public Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate, string ToDate)
        {
            throw new NotImplementedException();
        }
    }
}
