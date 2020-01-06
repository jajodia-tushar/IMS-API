using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IReportsService
    {
        Task<RAGStatusResponse> GetRAGStatus();
        Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string fromDate,string toDate);
        Task<MostConsumedItemsResponse> GetMostConsumedItems(string StartDate,string EndDate,int ItemsCount);
        Task<ItemsConsumptionReport> GetItemConsumptionStats(string startDate, string endDate);
        Task<StockStatusResponse> GetStockStatus(int pageNumber, int pageSize, string itemName);
        Task<ItemsAvailabilityResponse> GetItemsAvailability(string locationName, string locationCode, string colour,int pageNumber, int pageSize);
    }
}
