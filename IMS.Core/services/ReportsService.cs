using IMS.Core;
using IMS.Core.Validators;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ReportsService : IReportsService
    {
        private IReportsDbContext _reportsDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        private IShelfService _shelfService;
        private ITemporaryItemDbContext _temporaryItemDbContext;
        public ReportsService(IReportsDbContext reportsDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, IShelfService shelfService, ITemporaryItemDbContext temporaryItemDbContext)
        {
            this._reportsDbContext = reportsDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
            _shelfService = shelfService;
=           this._temporaryItemDbContext = temporaryItemDbContext;
        }
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string fromDate, string toDate)
        {
            
            ShelfWiseOrderCountResponse shelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    DateTime startDate = new DateTime();
                    DateTime endDate = new DateTime();
                    try
                    {
                        if (ReportsValidator.IsDateValid(fromDate, toDate, out startDate, out endDate))
                        {
                            
                            List<ShelfOrderStats> dateShelfOrderMappings = await PopulateListWithZeroValues(startDate, endDate);
                             _reportsDbContext.GetShelfWiseOrderCountByDate(startDate, endDate,dateShelfOrderMappings);

                            if (dateShelfOrderMappings == null || dateShelfOrderMappings.Count == 0)
                            {
                                shelfWiseOrderCountResponse.Status = Entities.Status.Failure;
                                shelfWiseOrderCountResponse.Error =
                                Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoShelfWiseOrderCount);

                            }
                            else
                            {
                                shelfWiseOrderCountResponse.Status = Entities.Status.Success;
                                shelfWiseOrderCountResponse.DateWiseShelfOrderCount = dateShelfOrderMappings;
                            }
                        }
                        else
                        {
                            shelfWiseOrderCountResponse.Status = Entities.Status.Failure;
                            shelfWiseOrderCountResponse.Error =
                            Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.DateRangeIsInvalid);
                        }
                    }
                    catch(Exception exception)
                    {
                        shelfWiseOrderCountResponse.Status = Entities.Status.Failure;
                        shelfWiseOrderCountResponse.Error =
                        Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.DateFormatInvalid);
                        new Task(() => {_logger.LogException(exception,"GetShelfWiseOrderCount", Severity.High, fromDate + ";" + toDate,
                        shelfWiseOrderCountResponse);
                        }).Start();
                    }
                }
                else
                {
                    shelfWiseOrderCountResponse.Status = Status.Failure;
                    shelfWiseOrderCountResponse.Error = 
                        Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, 
                        Constants.ErrorMessages.InvalidToken);
                }
            }
            catch(Exception exception)
            {
                shelfWiseOrderCountResponse.Status = Status.Failure;
                shelfWiseOrderCountResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetShelfWiseOrderCount", Severity.High, fromDate + ";" + toDate, shelfWiseOrderCountResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (shelfWiseOrderCountResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log("ShelfWiseOrderCount", shelfWiseOrderCountResponse, "GetShelfWiseOrderCount", shelfWiseOrderCountResponse.Status, severity, userId); }).Start();
            }
            return shelfWiseOrderCountResponse;
        }

        private async Task<List<ShelfOrderStats>> PopulateListWithZeroValues(DateTime startDate, DateTime toDate)
        {
            List<ShelfOrderStats> dateShelfOrderMappings = new List<ShelfOrderStats>();
            foreach (DateTime day in EachDay(startDate, toDate))
            {
                dateShelfOrderMappings.Add(
                    new ShelfOrderStats()
                    {
                        Date = day,
                        ShelfOrderCountMappings = await GetShelvesListWithOrderCountAsZero()
                    }
                );
            }
            return dateShelfOrderMappings;
        }
        private IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private async Task<List<ShelfOrderCountMapping>> GetShelvesListWithOrderCountAsZero()
        {
            var list = new List<ShelfOrderCountMapping>();
            ShelfResponse shelfResponse = new ShelfResponse();
            shelfResponse = await _shelfService.GetShelfList();
            foreach (var shelf in shelfResponse.Shelves)
            {
                list.Add(new ShelfOrderCountMapping()
                {
                    ShelfName = shelf.Name,
                    OrderCount = 0
                });
            }
            return list;
        }
        public async Task<MostConsumedItemsResponse> GetMostConsumedItems(string startDate, string endDate,int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    List<ItemQuantityMapping> itemQuantityMappings;
                    try
                    {
                        mostConsumedItemsResponse = ReportsValidator.ValidateDateAndItemsCount(startDate,endDate,itemsCount);
                        if (mostConsumedItemsResponse.Error == null)
                        {
                            itemQuantityMappings = await _reportsDbContext.GetMostConsumedItemsByDate(startDate,endDate,itemsCount);
                            if (itemQuantityMappings.Count != 0)
                            {
                                mostConsumedItemsResponse.Status = Status.Success;
                                mostConsumedItemsResponse.ItemQuantityMapping = itemQuantityMappings;
                            }
                            else
                            {
                                mostConsumedItemsResponse.Status = Status.Failure;
                                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                            }
                            return mostConsumedItemsResponse;
                        }
                        return mostConsumedItemsResponse;
                    }
                    catch (Exception exception)
                    {
                        mostConsumedItemsResponse.Status = Status.Failure;
                        mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                        new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", Severity.High, startDate+";"+endDate+";"+itemsCount, mostConsumedItemsResponse); }).Start();
                    }
                    return mostConsumedItemsResponse;
                }
                else
                {
                    mostConsumedItemsResponse.Status = Status.Failure;
                    mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", Severity.High, startDate + ";" + endDate + ";" + itemsCount, mostConsumedItemsResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.High;
                if (mostConsumedItemsResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(startDate + ";" + endDate + ";" + itemsCount, mostConsumedItemsResponse, "GetMostConsumedItems", mostConsumedItemsResponse.Status, severity, userId); }).Start();
            }
            return mostConsumedItemsResponse;
        }

        public async Task<RAGStatusResponse> GetRAGStatus()
        {
            var rAGStatusResponse = new RAGStatusResponse();
            rAGStatusResponse.Status = Status.Failure;
            int userId = -1;
            try
            {

                var warehouseRAGStatusList = await GetWarehouseRAGStatusList();
                var shelfRAGStatusList = await GetShelfRAGStatusList();
                if (warehouseRAGStatusList != null && shelfRAGStatusList != null && warehouseRAGStatusList.Count > 0 && shelfRAGStatusList.Count > 0)
                {
                    rAGStatusResponse.RAGStatusList = warehouseRAGStatusList;
                    rAGStatusResponse.RAGStatusList.AddRange(shelfRAGStatusList);
                    rAGStatusResponse.Status = Status.Success;
                    return rAGStatusResponse;
                }
                rAGStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.UnableToFetch);
                return rAGStatusResponse;
            }
            catch (Exception exception)
            {
                rAGStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetRAGStatus", Severity.Critical, "GetRAGStatus", rAGStatusResponse); }).Start();
                throw exception;
            }
            finally
            {
                Severity severity = Severity.No;
                if (rAGStatusResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("GetRAGStatus", rAGStatusResponse, "GetRAGStatus", rAGStatusResponse.Status, severity, -1); }).Start();
            }
        }

        private async Task<List<RAGStatus>> GetWarehouseRAGStatusList()
        {
            try
            {
                var warehouseRAGStatus = new RAGStatus() { Name = "Warehouse", Code = "WH", ColourCountMappings = InitializeColourCountMapping() };
                var ColourCountMappings = await _reportsDbContext.GetWarehouseRAGStatus();
                foreach (var colourCountMapping in ColourCountMappings)
                    warehouseRAGStatus.ColourCountMappings[(int)colourCountMapping.Colour].Count = colourCountMapping.Count;
                return new List<RAGStatus>() { { warehouseRAGStatus } };
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private async Task<List<RAGStatus>> GetShelfRAGStatusList()
        {
            try
            {
                var shelfRAGStatusList = new List<RAGStatus>();
                var dictShelfRAG = await _reportsDbContext.GetShelfRAGStatus();
                if (dictShelfRAG != null && dictShelfRAG.Count > 0)
                {
                    foreach (var keyValuePair in dictShelfRAG)
                    {
                        string[] locationDetail = keyValuePair.Key.Split(";");
                        var rAGStatus = new RAGStatus() { Code = locationDetail[0], Name = locationDetail[1], ColourCountMappings = InitializeColourCountMapping() };
                        //Modifying the Colour count if the colour exists in that location
                        foreach (var colourCountMapping in keyValuePair.Value)
                            rAGStatus.ColourCountMappings[(int)colourCountMapping.Colour].Count = colourCountMapping.Count;
                        shelfRAGStatusList.Add(rAGStatus);
                    }
                }
                return shelfRAGStatusList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private List<ColourCountMapping> InitializeColourCountMapping()
        {
            return new List<ColourCountMapping>() {
                { new ColourCountMapping() {Colour = Colour.Red,Count = 0} },
                { new ColourCountMapping() { Colour = Colour.Amber, Count = 0 } },
                { new ColourCountMapping() { Colour = Colour.Green, Count = 0 } }
            };
        }
        public async Task<ItemsConsumptionReport> GetItemConsumptionStats(string startDate, string endDate)
        {
            ItemsConsumptionReport itemConsumptionReport = new ItemsConsumptionReport();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    List<DateItemConsumption> dateItemConsumption;
                    if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate) && ReportsValidator.ValidateDate(startDate, endDate))
                    {
                        dateItemConsumption = await _reportsDbContext.GetItemsConsumptionReport(startDate, endDate);
                        if (dateItemConsumption != null)
                        {
                            itemConsumptionReport.Status = Status.Success;
                            dateItemConsumption = FillDatesInTheRange(startDate, endDate, dateItemConsumption);
                            itemConsumptionReport.ItemConsumptions = dateItemConsumption;
                        }
                        else
                        {
                            itemConsumptionReport.Status = Status.Failure;
                            itemConsumptionReport.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                        }
                        return itemConsumptionReport;
                    }
                    else
                    {
                        itemConsumptionReport.Status = Status.Failure;
                        itemConsumptionReport.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InvalidDate);
                    }
                    return itemConsumptionReport;
                }
                else
                {
                    itemConsumptionReport.Status = Status.Failure;
                    itemConsumptionReport.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                itemConsumptionReport.Status = Status.Failure;
                itemConsumptionReport.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetItemsConsumption", Severity.High, startDate + ";" + endDate, itemConsumptionReport); }).Start();
            }
            finally
            {
                Severity severity = Severity.High;
                if (itemConsumptionReport.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(startDate + ";" + endDate , itemConsumptionReport, "GetMostConsumedItems", itemConsumptionReport.Status, severity, userId); }).Start();
            }
            return itemConsumptionReport;
        }

        private List<DateItemConsumption> FillDatesInTheRange(string startDateString, string endDateString, List<DateItemConsumption> dateItemConsumptionList)
        {
            DateTime startDate = DateTime.ParseExact(startDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime endDate = DateTime.ParseExact(endDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            List<String> datesWithOrders = new List<String>();
            foreach (DateItemConsumption dateItemConsumption in dateItemConsumptionList)
            {
                datesWithOrders.Add(dateItemConsumption.Date);
            }
            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                string dateString = date.ToString("yyyy/MM/dd");
                if (!datesWithOrders.Contains(dateString))
                {
                    dateItemConsumptionList.Add(new DateItemConsumption() { Date = dateString, ItemsConsumptionCount=0});
                }
            }
            List<DateItemConsumption> sortedDateItemConsumptionList = dateItemConsumptionList.OrderBy(o => o.Date).ToList();
            return sortedDateItemConsumptionList;
        }
        public async Task<StockStatusResponse> GetStockStatus()
        {
            StockStatusResponse stockStatusResponse = new StockStatusResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    try
                    {
                        Dictionary<int,List<StoreColourQuantity>> stockStatus = await _reportsDbContext.GetStockStatus();
                        if (stockStatus!=null && stockStatus.Count != 0)
                        {
                            stockStatusResponse.Status = Status.Success;
                            stockStatusResponse.StockStatusList = await ToListFromDictionary(stockStatus);
                        }
                        else
                        {
                            stockStatusResponse.Status = Status.Failure;
                            stockStatusResponse.Error = new Error()
                            {
                                ErrorCode = Constants.ErrorCodes.NotFound,
                                ErrorMessage = Constants.ErrorMessages.NoItemsInStore
                            };
                        }
                    }
                    catch (Exception exception)
                    {
                        stockStatusResponse.Status = Status.Failure;
                        stockStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.UnableToShowStockStatus);
                        new Task(() => { _logger.LogException(exception, "Get Stock Status", Severity.Medium, null, stockStatusResponse); }).Start();
                    }
                }
                else
                {
                    stockStatusResponse.Status = Status.Failure;
                    stockStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                stockStatusResponse.Status = Status.Failure;
                stockStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.UnableToShowStockStatus);
                new Task(() => { _logger.LogException(exception, "Get Stock Status", Severity.Medium, null, stockStatusResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (stockStatusResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("Stock Status", stockStatusResponse, "Get Stock Status", stockStatusResponse.Status, severity, userId); }).Start();
            }
            return stockStatusResponse;
        }
<<<<<<< HEAD
        private List<StockStatusList> ToListFromDictionary(Dictionary<Item, List<StoreColourQuantity>> stockStatus)
=======

        public async Task<List<StockStatusList>> ToListFromDictionary(Dictionary<int, List<StoreColourQuantity>> stockStatusDict)
>>>>>>> completed stock status functionality
        {
            List<StockStatusList> stockStatusList = new List<StockStatusList>();
            foreach(int id in stockStatusDict.Keys)
            {
                StockStatusList stockStatusInstance = new StockStatusList();
                stockStatusInstance.Item = new Item();
                stockStatusInstance.StockStatus = new List<StoreColourQuantity>();
                stockStatusInstance.Item =await _temporaryItemDbContext.GetItemById(id);
                List<StoreColourQuantity> storeColourQuantities = new List<StoreColourQuantity>();
                storeColourQuantities = stockStatusDict[id];
                foreach(StoreColourQuantity listOfStockIterator in storeColourQuantities)
                {
                    stockStatusInstance.StockStatus.Add(listOfStockIterator);
                }
                stockStatusList.Add(stockStatusInstance);
            }
            return stockStatusList;
        }
    }
}
