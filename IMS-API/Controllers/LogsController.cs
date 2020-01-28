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
    //[Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private ILogsService _logsService;
        private ILogManager _logger;
        public LogsController(ILogsService logsService, ILogManager logManager)
        {
            _logsService = logsService;
            _logger = logManager;
        }

        /// <summary>
        /// Return Log History 
        /// </summary>
        /// <returns>Log History</returns>
        /// <response code="200">Returns Log History</response>
        /// <response code="404">Log History Not Found</response>
        // GET: api/Logs
        [Route("api/Logs")]
        [HttpGet]
        public async Task<LogsResponse> GetLogs()
        {
            LogsResponse dtoLogsResponse = null;
            try
            {
                IMS.Entities.LogsResponse doLogsResponse = await _logsService.GetLogsRecord();
                dtoLogsResponse = LogsTranslator.ToDataContractsObject(doLogsResponse);

            }
            catch (Exception exception)
            {
                dtoLogsResponse = new IMS.Contracts.LogsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetLogs", IMS.Entities.Severity.Critical,null,dtoLogsResponse); }).Start();
            }
            return dtoLogsResponse;
        }
        [Route("api/ActivityLogs")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActivityLogsResponse> GetActivityLogs(int? pageNumber,int? pageSize,string fromDate = null,string toDate = null)
        {
            ActivityLogsResponse contractsActivityLogResponse = null;
            try
            {
                IMS.Entities.ActivityLogsReponse activityLogsReponse = await _logsService.GetActivityLogs(pageNumber,pageSize,fromDate,toDate);
                contractsActivityLogResponse = LogsTranslator.ToDataContractsObject(activityLogsReponse);
            }
            catch(Exception exception)
            {
                contractsActivityLogResponse = new IMS.Contracts.ActivityLogsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetActivityLogs", IMS.Entities.Severity.High, null, contractsActivityLogResponse); }).Start();
            }
            return contractsActivityLogResponse;
        }
    }
}
