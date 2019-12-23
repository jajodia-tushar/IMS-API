using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IReportsService
    {
        Task<RAGStatusResponse> GetRAGStatus();
        Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate,string ToDate);
        Task<MostConsumedItemsResponse> GetMostConsumedItems(string StartDate,string EndDate,int ItemsCount);
        Task<ItemsConsumptionReport> GetItemConsumptionStats(string startDate, string endDate);
    }
}
