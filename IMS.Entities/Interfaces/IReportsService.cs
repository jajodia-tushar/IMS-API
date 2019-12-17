using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IReportsService
    {
        Task<RAGStatusResponse> GetRAGStatus();
        Task<ShelfWiseNumberOfEmployeeOrderResponse> GetWiseNumberOfEmployeeOrder(string FromDate,string ToDate);
    }
}
