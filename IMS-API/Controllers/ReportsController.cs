using System;
using System.Collections.Generic;
using System.Linq;
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
        public ReportsController(IReportsService reportsService, ILogManager logManager)
        {
            _reportsService = reportsService;
            _logger = logManager;

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

        //http://localhost:55462/api/Reports/GetShelfWiseOrderCount?FromDate=10/12/2019&ToDate=19/12/2019
        [Route("GetShelfWiseOrderCount")]
        [HttpGet]
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate,string ToDate)
        {
            ShelfWiseOrderCountResponse dtoShelfWiseOrderCountResponse = new ShelfWiseOrderCountResponse();
            try
            {
                IMS.Entities.ShelfWiseOrderCountResponse doShelfWiseOrderCountResponse =
                await _reportsService.GetShelfWiseOrderCountAsync(FromDate, ToDate);
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
                new Task(() => { _logger.LogException(exception, "GetAllVendors", IMS.Entities.Severity.High, null, dtoShelfWiseOrderCountResponse); }).Start();
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
                new Task(() => { _logger.LogException(exception, "Get Item Consumption Report", IMS.Entities.Severity.High, startDate + ";" + endDate, itemConsumptionReport); }).Start();
            }
            return itemConsumptionReport;
        }
    }
}
