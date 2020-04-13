using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ReportsController : ControllerBase
    {

        private IReportsService _reportsService;
        private ILogManager _logger;
        public ReportsController(IReportsService reportsService , ILogManager logger)
        {
            _reportsService = reportsService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve RAG(Red, Amber, Green) status of every inventory location
        /// </summary>
        /// <returns>RAG count of every shelf</returns>
        /// <response code="200">Returns the RAG count of every inventory location if the system is unable to fetch any of the location then it returns failure</response>
        [Route("GetRAGStatus")]
        [HttpGet]
        public async Task<RAGStatusResponse> GetRAGStatus()
        {
            RAGStatusResponse rAGStatusResponse = new RAGStatusResponse();
            try
            {
                IMS.Entities.RAGStatusResponse doRAGStatusResponse = await _reportsService.GetRAGStatus();
                rAGStatusResponse = RAGStatusTranslator.ToDataContractObject(doRAGStatusResponse);
            }
            catch (Exception exception)
            {
                rAGStatusResponse.Status = Status.Failure;
                rAGStatusResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };
                new Task(() => { _logger.LogException(exception, "GetRAGStatus", IMS.Entities.Severity.Critical, "GetRAGStatus", rAGStatusResponse); }).Start();
            }
            return rAGStatusResponse;
        }


        /// <summary>
        /// Retrieve Frequently used "n" items in given date range 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="itemsCount"></param>
        /// <returns>Most frequently used "n" items with their quantity</returns>
        /// <response code="200">Returns item with their quantity used if input is valid otherwise it returns status failure if input is not valid</response>
        [Route("GetMostConsumedItems")]
        [HttpGet]
        public async Task<MostConsumedItemsResponse> GetMostConsumedItems(string startDate, string endDate, int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = null;
            try
            {
                IMS.Entities.MostConsumedItemsResponse mostConsumedItemsResponseEntity = await _reportsService.GetMostConsumedItems(startDate,endDate,itemsCount);
                mostConsumedItemsResponse = ReportsTranslator.ToDataContractsObject(mostConsumedItemsResponseEntity);
            }
            catch (Exception exception)
            {
                mostConsumedItemsResponse = new IMS.Contracts.MostConsumedItemsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", IMS.Entities.Severity.High, startDate+";"+endDate+";"+itemsCount, mostConsumedItemsResponse); }).Start();
            }
            return mostConsumedItemsResponse;
        }

        //http://localhost:55462/api/Reports/GetShelfWiseOrderCount?FromDate=20191210&ToDate=20191219
        //yyyymmdd
        /// <summary>
        /// Returns shelf wise order count by date
        /// </summary>
        /// <returns>List of date with shelf wise order count</returns>
        /// <response code="200">Returns the shelf wise order count along with status success </response>
        /// <response code="404">If Shelf Wise Order Count is not Available </response>
        /// <response code="401">If token is Invalid</response>
        /// <response code="403">If Username and Password credentials are not of Admin and SuperAdmin</response>
        /// <response code="400">Given Date Range Is Invalid or Given Date Format is Invalid</response>
        [Route("GetShelfWiseOrderCount")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string fromDate,string toDate)
        {
            
            ShelfWiseOrderCountResponse dtoShelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            try
            {
                IMS.Entities.ShelfWiseOrderCountResponse doShelfWiseOrderCountResponse =
                await _reportsService.GetShelfWiseOrderCount(fromDate, toDate);
                dtoShelfWiseOrderCountResponse = ReportsTranslator.ToDataContractsObject(doShelfWiseOrderCountResponse);
            }
            catch (Exception exception)
            {
                dtoShelfWiseOrderCountResponse = new IMS.Contracts.ShelfWiseOrderCountResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Get Shelf Wise Order Count", 
                    IMS.Entities.Severity.High, null, dtoShelfWiseOrderCountResponse); }).Start();
            }
            return dtoShelfWiseOrderCountResponse;
        }

        /// <summary>
        /// Retrieve datewise item consumption count between a given date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Datewise item consumption count</returns>
        /// <response code="200">Returns datewise item consumption count if input is valid otherwise it returns status failure</response>
        [Route("GetItemConsumption")]
        [HttpGet]
        public async Task<ItemsConsumptionReport> GetItemsConsumption(string startDate, string endDate)
        {
            ItemsConsumptionReport itemConsumptionReport = null;
            try
            {
                IMS.Entities.ItemsConsumptionReport itemConsumptionReportEntity = await _reportsService.GetItemConsumptionStats(startDate, endDate);
                itemConsumptionReport = ReportsTranslator.ToDataContractsObject(itemConsumptionReportEntity);
            }
            catch (Exception exception)
            {
                itemConsumptionReport = new IMS.Contracts.ItemsConsumptionReport()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetItemsConsumption", IMS.Entities.Severity.High, startDate + ";" + endDate, itemConsumptionReport); }).Start();
            }
            return itemConsumptionReport;
        }

        /// <summary>
        /// Retrieve the stock status at any given point of time
        /// </summary>
        /// <returns>Returns the whole items list with the stock status at any given point of time</returns>
        /// <response code="200">Retrieve the stock status at any given point of time</response>
        // GET: api/Default/5
        [HttpGet("GetStockStatus", Name = "GetStockStatus")]
        public async Task<StockStatusResponse> GetStockStatus(int pageNumber,int pageSize, string itemName,string itemIds)
        {
            StockStatusResponse stockStatusResponse = null;
            try
            {
                IMS.Entities.StockStatusResponse entityStockStatusResponse = await _reportsService.GetStockStatus(pageNumber,pageSize,itemName,itemIds);
                stockStatusResponse = StockStatusTranslator.ToDataContractsObject(entityStockStatusResponse);
            }
            catch (Exception exception)
            {
                stockStatusResponse = new IMS.Contracts.StockStatusResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetStockStatus", IMS.Entities.Severity.Critical, "GetStockStatus", stockStatusResponse); }).Start();
            }
            return stockStatusResponse;
        }

        /// <summary>
        /// Retrieve Location and Colour based items list
        /// </summary>
        /// <param name="locationName"></param>
        /// <param name="locationCode"></param>
        /// <param name="colour"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Location and Colour based Items list</returns>
        /// <response code="200">Returns Location and colour based items list count if input is valid otherwise it returns status failure</response>
        [Route("GetItemsAvailability")]
        [HttpGet]
        public async Task<ItemsAvailabilityResponse> GetItemsAvailability(string locationName, string locationCode, string colour, int? pageNumber = null, int? pageSize = null)
        {
            var itemsAvailabilityResponse = new ItemsAvailabilityResponse();
            try
            {
                int currentPageNumber = pageNumber ?? 1;
                int currentPageSize = pageSize ?? 10;
                IMS.Entities.ItemsAvailabilityResponse doItemsAvailabilityResponse = await _reportsService.GetItemsAvailability(locationName, locationCode, colour,currentPageNumber,currentPageSize);
                itemsAvailabilityResponse = ReportsTranslator.ToDataContractsObject(doItemsAvailabilityResponse);
            }
            catch (Exception exception)
            {
                itemsAvailabilityResponse.Status = Status.Failure;
                itemsAvailabilityResponse.Error = Translator.ToDataContractsObject(Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError));
                new Task(() => { _logger.LogException(exception, "GetItemsAvailability", IMS.Entities.Severity.High, locationName + ";" + locationCode + ";" + colour, itemsAvailabilityResponse); }).Start();
            }
            return itemsAvailabilityResponse;
        }

        /// <summary>
        /// Get Per Day Consumption Report
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List Of Date with Item Quantity Mapping</returns>
        /// <response code="200">Returns List Of Date With Item Quantity Mapping Within Date Range if Input is valid otherwise it returns status failure</response>
        [Route("GetItemConsumptionReports")]
        [HttpGet]
        public async Task<DateWiseItemsConsumption> GetItemsConsumptionReport(int pageNumber, int pageSize,string fromDate, string toDate)
        {
            DateWiseItemsConsumption dateWiseItemsConsumption = null;
            try
            {
                IMS.Entities.DateWiseItemsConsumption dateWiseItemsConsumptionEntity = await _reportsService.GetItemConsumptionReports(pageNumber,pageSize,fromDate, toDate);
                dateWiseItemsConsumption = ReportsTranslator.ToDataContractsObject(dateWiseItemsConsumptionEntity);
            }
            catch (Exception exception)
            {
                dateWiseItemsConsumption = new IMS.Contracts.DateWiseItemsConsumption()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetItemConsumptionReports", IMS.Entities.Severity.High, fromDate + ";" + toDate, dateWiseItemsConsumption); }).Start();
            }
            return dateWiseItemsConsumption;
        }
        /// <summary>
        /// Retrieve datewise item consumption details between a given date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Datewise item consumption count</returns>
        /// <response code="200">Returns datewise item consumption details if input is valid otherwise it returns status failure</response>
        [Route("GetDateWiseItemConsumptionDetails")]
        [HttpGet]
        public async Task<ItemConsumptionDetailsResponse> GetDateWiseItemConsumptionDetails(string startDate, string endDate, int pageNumber, int pageSize)
        {
            ItemConsumptionDetailsResponse itemConsumptionDetailsReport = null;
            try
            {
                IMS.Entities.ItemConsumptionDetailsResponse itemConsumptionDetailsReportEntity = await _reportsService.GetDateWiseItemConsumptionDetails(startDate, endDate, pageNumber, pageSize);
                itemConsumptionDetailsReport = ReportsTranslator.ToDataContractsObject(itemConsumptionDetailsReportEntity);
            }
            catch (Exception exception)
            {
                itemConsumptionDetailsReport = new IMS.Contracts.ItemConsumptionDetailsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetDateWiseItemConsumptionDetails", IMS.Entities.Severity.High, startDate + ";" + endDate, itemConsumptionDetailsReport); }).Start();
            }
            return itemConsumptionDetailsReport;
        }
        /// <summary>
        /// Retrieve datewise bulk order details between a given date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Datewise  bulk order details</returns>
        /// <response code="200">Returns datewise  bulk order details if input is valid otherwise it returns status failure</response>
        [Route("GetEmployeeBulkOrders")]
        [HttpGet]
        public async Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrders(string startDate, string endDate, int pageNumber, int pageSize)
        {
            EmployeeBulkOrdersResponse employeeBulkOrdersResponse = null;
            try
            {
                IMS.Entities.EmployeeBulkOrdersResponse employeeBulkOrdersResponseEntity = await _reportsService.GetEmployeeBulkOrders(startDate, endDate, pageNumber, pageSize);
                employeeBulkOrdersResponse = ReportsTranslator.ToDataContractsObject(employeeBulkOrdersResponseEntity);
            }
            catch (Exception exception)
            {
                employeeBulkOrdersResponse = new IMS.Contracts.EmployeeBulkOrdersResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetDateEmployeeBulkOrders", IMS.Entities.Severity.High, startDate + ";" + endDate, employeeBulkOrdersResponse); }).Start();
            }
            return employeeBulkOrdersResponse;
        }
    }
}
