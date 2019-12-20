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
            this._logger = logManager;
        }

        [Route("GetRAGStatus")]
        [HttpGet]
        public async Task<RAGStatusResponse> GetRAGStatus()
        {

            RAGStatusResponse rAGStatusResponse = new RAGStatusResponse();
            List<ColourCountMapping> c = new List<ColourCountMapping>();
            rAGStatusResponse.Error = null;
            rAGStatusResponse.Status = Status.Success;
            c.Add(new ColourCountMapping() { Colour = Colour.Red, Count = 9 });
            c.Add(new ColourCountMapping() { Colour = Colour.Amber, Count = 10 });
            c.Add(new ColourCountMapping() { Colour = Colour.Green, Count = 13 });
            rAGStatusResponse.RAGStatusList = new List<RAGStatus>();
            rAGStatusResponse.RAGStatusList.Add(
                new RAGStatus()
                {
                    Name = "floor-1",
                    Code = "A",
                    ColourCountMappings = c
                });
            rAGStatusResponse.RAGStatusList.Add(
               new RAGStatus()
               {
                   Name = "floor-2",
                   Code = "B",
                   ColourCountMappings = c
               });
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

        [Route("GetShelfWiseOrderCount")]
        [HttpGet]
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate,string ToDate)
        {
           
            throw new NotImplementedException();
        }
    }
}
