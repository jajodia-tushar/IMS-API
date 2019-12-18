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
        Task<List<DateItemConsumption>> GetItemsConsumptionReport(string startDate,string endDate);
        Task<List<DateShelfOrderMapping>> GetShelfWiseOrderCountByDate(DateTime StartDate,DateTime ToDate );
    }

}
