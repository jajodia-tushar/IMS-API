using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IReportsDbContext
    {
        Task<List<ItemQuantityMapping>> GetMostConsumedItemsByDate(string startDate, string endDate, int itemsCount);
        Task<Dictionary<string, List<ColourCountMapping>>> GetShelfRAGStatus();
        Task<List<ColourCountMapping>> GetWarehouseRAGStatus();
<<<<<<< HEAD
        Task<List<DateItemConsumption>> GetItemsConsumptionReport(string startDate,string endDate);
        void GetShelfWiseOrderCountByDate(DateTime startDate,DateTime toDate, List<ShelfOrderStats> shelfOrderStats );
=======
        Task<Dictionary<int, List<StoreColourQuantity>>> GetStockStatus();
>>>>>>> Added functionality of sending total names of stores in the response
    }

}
