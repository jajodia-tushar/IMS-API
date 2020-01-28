using IMS.Core;
using IMS.Core.Validators;
using IMS.DataLayer.Dto;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
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
        private IItemDbContext _itemDbContext;

        public ReportsService(IReportsDbContext reportsDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, IShelfService shelfService, IItemDbContext itemDbContext)
        {
            this._reportsDbContext = reportsDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._shelfService = shelfService;
            this._httpContextAccessor = httpContextAccessor;
            this._itemDbContext = itemDbContext;
        }
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string fromDate, string toDate)
        {

            ShelfWiseOrderCountResponse shelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    DateTime startDate = new DateTime();
                    DateTime endDate = new DateTime();
                    try
                    {
                        if (ReportsValidator.IsDateValid(fromDate, toDate, out startDate, out endDate))
                        {

                            List<ShelfOrderStats> dateShelfOrderMappings = await PopulateListWithZeroValues(startDate, endDate);
                            _reportsDbContext.GetShelfWiseOrderCountByDate(startDate, endDate, dateShelfOrderMappings);

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
                    catch (Exception exception)
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
            catch (Exception exception)
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
        public async Task<MostConsumedItemsResponse> GetMostConsumedItems(string startDate, string endDate, int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    List<ItemQuantityMapping> itemQuantityMappings;
                    try
                    {
                        mostConsumedItemsResponse = ReportsValidator.ValidateDateAndItemsCount(startDate, endDate, itemsCount);
                        if (mostConsumedItemsResponse.Error == null)
                        {
                            itemQuantityMappings = await _reportsDbContext.GetMostConsumedItemsByDate(startDate, endDate, itemsCount);
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
                        new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", Severity.High, startDate + ";" + endDate + ";" + itemsCount, mostConsumedItemsResponse); }).Start();
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
                Severity severity = Severity.No;
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
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
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
                new Task(() => { _logger.Log(startDate + ";" + endDate, itemConsumptionReport, "GetMostConsumedItems", itemConsumptionReport.Status, severity, userId); }).Start();
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
                    dateItemConsumptionList.Add(new DateItemConsumption() { Date = dateString, ItemsConsumptionCount = 0 });
                }
            }
            List<DateItemConsumption> sortedDateItemConsumptionList = dateItemConsumptionList.OrderBy(o => o.Date).ToList();
            return sortedDateItemConsumptionList;
        }
        public async Task<StockStatusResponse> GetStockStatus(int pageNumber, int pageSize, string itemName,string itemIds)
        {
            StockStatusResponse stockStatusResponse = new StockStatusResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    if(pageSize<=0||pageNumber<=0)
                    {
                        stockStatusResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return stockStatusResponse;
                    }
                    if(String.IsNullOrEmpty(itemName))
                    {
                        itemName = "";
                    }
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    try
                    {
                        ItemStockStatusDto stockStatus = await _reportsDbContext.GetStockStatus(limit,offset,itemName);
                        if (stockStatus != null && stockStatus.StockStatus.Count != 0)
                        {
                            stockStatusResponse.Status = Status.Success;
                            stockStatusResponse.PagingInfo = new PagingInfo()
                            {
                                TotalResults = stockStatus.PagingInfo.TotalResults,
                                PageNumber = pageNumber,
                                PageSize = pageSize
                            };
                            stockStatusResponse.StockStatusList = await ToListFromDictionary(stockStatus);
                            if (!String.IsNullOrEmpty(itemIds))
                            {
                                List<int> itemsId = new List<int>();
                                string[] itemIdsList = itemIds.Split(",");
                                foreach (string itemId in itemIdsList)
                                {
                                    try
                                    {
                                        itemsId.Add(Convert.ToInt32(itemId));
                                    }
                                    catch (Exception e)
                                    {
                                        stockStatusResponse.Status = Status.Failure;
                                        stockStatusResponse.Error = new Error()
                                        {
                                            ErrorCode = Constants.ErrorCodes.NotFound,
                                            ErrorMessage = Constants.ErrorMessages.InvalidItemId
                                        };
                                        return stockStatusResponse;
                                    }
                                }
                                stockStatusResponse.StockStatusList = stockStatusResponse.StockStatusList.Where(o => itemsId.Contains(o.Item.Id)).ToList();
                            }
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
                        new Task(() => { _logger.LogException(exception, "GetStockStatus", Severity.Medium, "GetStockStatus", stockStatusResponse); }).Start();
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
                new Task(() => { _logger.LogException(exception, "GetStockStatus", Severity.Medium, "GetStockStatus", stockStatusResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (stockStatusResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("GetStockStatus", stockStatusResponse, "GetStockStatus", stockStatusResponse.Status, severity, userId); }).Start();
            }
            return stockStatusResponse;
        }
        public async Task<List<ItemStockStatus>> ToListFromDictionary(ItemStockStatusDto stockStatus)
        {
            List<ItemStockStatus> itemStockStatusList = new List<ItemStockStatus>();
            foreach(Item item in stockStatus.Items)
            {
                ItemStockStatus itemStockStatusInstance = new ItemStockStatus();
                itemStockStatusInstance.Item = item;
                itemStockStatusInstance.StoreStatus = new List<StockStatus>();
                List<StockStatus> stockStatusList = new List<StockStatus>();
                if(stockStatus.StockStatus.ContainsKey(item.Id))
                {
                    stockStatusList = stockStatus.StockStatus[item.Id];
                    foreach (StockStatus stockStatusIterator in stockStatusList)
                    {
                        itemStockStatusInstance.StoreStatus.Add(stockStatusIterator);
                    }
                }
                itemStockStatusList.Add(itemStockStatusInstance);
            }
            return itemStockStatusList;
        }

        public async Task<ItemsAvailabilityResponse> GetItemsAvailability(string locationName, string locationCode, string colour,int pageNumber,int pageSize)
        {
            var dtoItemsAvailabilityResponse = new ItemsAvailabilityResponse();
            PagingInfo pagingInfo = new PagingInfo();
            dtoItemsAvailabilityResponse.Status = Status.Failure;
            int userId = -1;
            try
            {
                object inputColour;
                if (pageSize <= 0 || pageNumber <= 0)
                    throw new InvalidPagingInfo();
                pagingInfo.PageNumber = pageNumber;
                pagingInfo.PageSize = pageSize;
                if (!String.IsNullOrEmpty(locationName) && !String.IsNullOrEmpty(locationCode) && !String.IsNullOrEmpty(colour) && Enum.TryParse(typeof(Colour), colour, true, out inputColour))
                {
                    if (locationName.ToUpper() == "WAREHOUSE")
                    {
                       var itemsAvailabilityResponse = await _reportsDbContext.GetWarehouseAvailability(inputColour.ToString(),pageNumber,pageSize);
                        if(itemsAvailabilityResponse.ItemQuantityMappings!=null && itemsAvailabilityResponse.ItemQuantityMappings.Count > 0)
                        {

                            itemsAvailabilityResponse.Status = Status.Success;
                            dtoItemsAvailabilityResponse = itemsAvailabilityResponse;
                            return dtoItemsAvailabilityResponse;
                        }
                        dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                        return dtoItemsAvailabilityResponse;
                    }
                    else
                    {
                        var shelfResponse = new ShelfResponse();
                        shelfResponse = await _shelfService.GetShelfByShelfCode(locationCode);
                        if (shelfResponse.Status == Status.Success)
                        {
                            var itemsAvailabilityResponse = await _reportsDbContext.GetShelfAvailability(shelfResponse.Shelves[0].Id, inputColour.ToString(),pageNumber,pageSize);
                            if (itemsAvailabilityResponse.ItemQuantityMappings != null && itemsAvailabilityResponse.ItemQuantityMappings.Count > 0)
                            {
                                itemsAvailabilityResponse.Status = Status.Success;
                                dtoItemsAvailabilityResponse = itemsAvailabilityResponse;
                                return dtoItemsAvailabilityResponse;
                            }
                            dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                            return dtoItemsAvailabilityResponse;
                        }
                        dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.LocationNotfound);
                        return dtoItemsAvailabilityResponse;
                    }
                }
                dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidInput);
                return dtoItemsAvailabilityResponse;
            }
            catch (CustomException e)
            {
                dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "GetItemsAvailability", Severity.High, locationName + ";" + locationCode + ";" + colour + ";" + pageNumber + ";" + pageSize, dtoItemsAvailabilityResponse); }).Start();
            }
            catch (Exception exception)
            {
                dtoItemsAvailabilityResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetItemsAvailability", Severity.High, locationName+";"+locationCode+";"+colour + ";" + pageNumber + ";" + pageSize, dtoItemsAvailabilityResponse); }).Start();
                throw exception;
            }
            finally
            {
                Severity severity = Severity.No;
                if (dtoItemsAvailabilityResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log("GetItemsAvailability", dtoItemsAvailabilityResponse, "GetItemsAvailability", dtoItemsAvailabilityResponse.Status, severity, userId); }).Start();
            }
            return dtoItemsAvailabilityResponse;
        }

        public async Task<DateWiseItemsConsumption> GetItemConsumptionReports(int pageNumber, int pageSize,string fromDate, string toDate)
        {
            DateWiseItemsConsumption dateWiseItemsConsumption = new DateWiseItemsConsumption();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate) && ReportsValidator.ValidateDate(fromDate, toDate))
                    {
                        if(pageNumber<0||pageSize<0)
                        {
                            dateWiseItemsConsumption.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                            return dateWiseItemsConsumption;
                        }
                        if(pageNumber==0||pageSize==0)
                        {
                            pageSize = 10;
                            pageNumber = 1;
                        }
                        int limit = pageSize;
                        int offset = (pageNumber - 1) * pageSize;
                        dateWiseItemsConsumption = await _reportsDbContext.GetItemsConsumptionReports(limit,offset, fromDate, toDate);
                        if (dateWiseItemsConsumption.DateItemMapping != null)
                        {
                            dateWiseItemsConsumption.PagingInfo.PageNumber = pageNumber;
                            dateWiseItemsConsumption.PagingInfo.PageSize = pageSize;
                            dateWiseItemsConsumption.Status = Status.Success;
                        }
                        else
                        {
                            dateWiseItemsConsumption.Status = Status.Failure;
                            dateWiseItemsConsumption.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                        }
                        return dateWiseItemsConsumption;
                    }
                    else
                    {
                        dateWiseItemsConsumption.Status = Status.Failure;
                        dateWiseItemsConsumption.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InvalidDate);
                    }
                    return dateWiseItemsConsumption;
                }
                else
                {
                    dateWiseItemsConsumption.Status = Status.Failure;
                    dateWiseItemsConsumption.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                dateWiseItemsConsumption.Status = Status.Failure;
                dateWiseItemsConsumption.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetItemsConsumptionReports", Severity.High, fromDate + ";" + toDate, dateWiseItemsConsumption); }).Start();
            }
            finally
            {
                Severity severity = Severity.High;
                if (dateWiseItemsConsumption.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(fromDate + ";" + toDate, dateWiseItemsConsumption, "GetItemsConsumptionReports", dateWiseItemsConsumption.Status, severity, userId); }).Start();
            }
            return dateWiseItemsConsumption;
        }
        public async Task<ItemConsumptionDetailsResponse> GetDateWiseItemConsumptionDetails(string startDate, string endDate, int pageNumber, int pageSize)
        {
            ItemConsumptionDetailsResponse itemConsumptionResponse = new ItemConsumptionDetailsResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    if (pageSize <= 0 || pageNumber <= 0)
                    {
                        itemConsumptionResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return itemConsumptionResponse;
                    }
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    itemConsumptionResponse = await _reportsDbContext.GetDateWiseItemConsumptionDetails(startDate, endDate, limit, offset);
                    if (itemConsumptionResponse.DateWiseItemConsumptionDetails != null && itemConsumptionResponse.DateWiseItemConsumptionDetails.Count>0)
                    {
                        itemConsumptionResponse.PagingInfo.PageNumber = pageNumber;
                        itemConsumptionResponse.PagingInfo.PageSize = pageSize;
                        itemConsumptionResponse.Status = Status.Success;
                    }
                    else
                    {
                        itemConsumptionResponse.Status = Status.Failure;
                        itemConsumptionResponse.Error = new Error()
                        {
                            ErrorCode = Constants.ErrorCodes.NotFound,
                            ErrorMessage = Constants.ErrorMessages.RecordNotFound
                        };
                    }
                }
                else
                {
                    itemConsumptionResponse.Status = Status.Failure;
                    itemConsumptionResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                itemConsumptionResponse.Status = Status.Failure;
                itemConsumptionResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.UnableToShowStockStatus);
                new Task(() => { _logger.LogException(exception, "GetItemCosumptionDetails", Severity.Medium, "GetItemCosumptionDetails", itemConsumptionResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (itemConsumptionResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("GetItemCosumptionDetails", itemConsumptionResponse, "GetItemCosumptionDetails", itemConsumptionResponse.Status, severity, userId); }).Start();
            }
            return itemConsumptionResponse;
        }
    }
}
