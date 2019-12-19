using IMS.Core.Validators;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ReportsService : IReportsService
    {
        private IReportsDbContext _reportsDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;

        public ReportsService(IReportsDbContext reportsDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._reportsDbContext = reportsDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
        }

        public Task<RAGStatusResponse> GetRAGStatus()
        {
            throw new NotImplementedException();
        }

        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCountAsync(string fromDate, string toDate)
        {
            
            ShelfWiseOrderCountResponse shelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (IsDateValid(fromDate, toDate, out startDate, out endDate))
                {
                    List<DateShelfOrderMapping> dateShelfOrderMappings =
                    await _reportsDbContext.GetShelfWiseOrderCountByDate(startDate, endDate);

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
                shelfWiseOrderCountResponse.Status = Status.Failure;
                shelfWiseOrderCountResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Getting Shelf Wise Order Count", Severity.High, null, shelfWiseOrderCountResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (shelfWiseOrderCountResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log("ShelfWiseOrderCount",shelfWiseOrderCountResponse, "Getting Shelf Wise Order Count", shelfWiseOrderCountResponse.Status, severity, -1); }).Start();
            }
            return shelfWiseOrderCountResponse;
        }

        private bool IsDateValid(string fromDate, string toDate, out DateTime startDate, out DateTime endDate)
        {
            
            Regex regex = new Regex(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$");
            
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                Match match = regex.Match(fromDate);
                Match match1 = regex.Match(toDate);
                if (match.Success && match1.Success)
                {
                    fromDate = fromDate.Substring(6, 2) + "/" + fromDate.Substring(4, 2) + "/" + fromDate.Substring(0, 4);
                    toDate = toDate.Substring(6, 2) + "/" + toDate.Substring(4, 2) + "/" + toDate.Substring(0, 4);
                    startDate = ConvertStringDateToDateTimeObject(fromDate);
                    endDate = ConvertStringDateToDateTimeObject(toDate);
                    if (IsDateRangeIsValid(startDate, endDate))
                    {
                        return true;
                    }
                }  
            }
            startDate = DateTime.Now;
            endDate = DateTime.Now;
            return false;
        }

        private bool IsDateRangeIsValid(DateTime startDate, DateTime endDate)
        {
            int value = DateTime.Compare(startDate, endDate);
            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                return false;
            }
            else if (value < 0)
            {
                return true;
            }
            return false;
        }

        private DateTime ConvertStringDateToDateTimeObject(string date)
        {
            DateTime Date;
            int day = Convert.ToInt32(date.Split('/')[0]);
            int month = Convert.ToInt32(date.Split('/')[1]);
            int year = Convert.ToInt32(date.Split('/')[2]);
            if (ValidateDate(day, month, year) == true)
            {
                Date = new DateTime(year, month, day);
                return Date;
            }
            return new DateTime();
        }

        private bool ValidateDate(int day, int month, int year)
        {
            int numberOfDaysInMonth = DateTime.DaysInMonth(year, month);
            if (month > 0 && month <= 12)
            {
                if (day > 0 && day <= numberOfDaysInMonth)
                {
                    return true;
                }
            }
            return false;
        }
    }
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

        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCountAsync(string FromDate, string ToDate)
        {
            //      dd/mm/yyyy
            ShelfWiseOrderCountResponse shelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            int dd = Convert.ToInt32(FromDate.Split('/')[0]);
            int mm = Convert.ToInt32(FromDate.Split('/')[1]);
            int yyyy = Convert.ToInt32(FromDate.Split('/')[2]);
            DateTime StartDate = new DateTime(yyyy, mm, dd);

            dd = Convert.ToInt32(ToDate.Split('/')[0]);
            mm = Convert.ToInt32(ToDate.Split('/')[1]);
            yyyy = Convert.ToInt32(ToDate.Split('/')[2]);

            DateTime EndDate = new DateTime(yyyy, mm, dd);
            try
            {
                List<DateShelfOrderMapping> dateShelfOrderMappings =
              await _reportsDbContext.GetShelfWiseOrderCountByDate(StartDate, EndDate);
                if(dateShelfOrderMappings == null || dateShelfOrderMappings.Count==0)
                {
                    shelfWiseOrderCountResponse.Status = Entities.Status.Failure;
                    shelfWiseOrderCountResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.NoShelfWiseOrderCount

                    };
                }
                else
                {
                    shelfWiseOrderCountResponse.Status = Entities.Status.Success;
                    shelfWiseOrderCountResponse.DateWiseShelfOrderCount = dateShelfOrderMappings;
                }
            }
            catch(Exception exception)
            {
                shelfWiseOrderCountResponse.Status = Status.Failure;
                shelfWiseOrderCountResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Getting Shelf Wise Order Count", Severity.High, null, shelfWiseOrderCountResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (shelfWiseOrderCountResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log("ShelfWiseOrderCount",shelfWiseOrderCountResponse, "Getting Shelf Wise Order Count", shelfWiseOrderCountResponse.Status, severity, -1); }).Start();
            }
            return shelfWiseOrderCountResponse;
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
            return dateItemConsumptionList;
        }
    }
}
